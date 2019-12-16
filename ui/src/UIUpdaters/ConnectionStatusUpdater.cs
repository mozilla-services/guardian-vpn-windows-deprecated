// <copyright file="ConnectionStatusUpdater.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FirefoxPrivateNetwork.WireGuard;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Monitors connection health by querying statistics the VPN tunnel service and changes UI accordingly.
    /// </summary>
    internal class ConnectionStatusUpdater
    {
        private readonly ViewModels.MainWindowViewModel viewModel;
        private Thread updater = null;
        private CancellationTokenSource updaterCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStatusUpdater"/> class.
        /// </summary>
        /// <param name="vm">View model for the main window.</param>
        public ConnectionStatusUpdater(ViewModels.MainWindowViewModel vm)
        {
            viewModel = vm;
        }

        /// <summary>
        /// Starts the connection health monitor thread.
        /// </summary>
        public void StartThread()
        {
            if (updater != null && updater.IsAlive)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();

            updater = new Thread(() => PollConnectionStatus(updaterCancellationTokenSource.Token))
            {
                IsBackground = true,
            };
            updater.Start();
        }

        /// <summary>
        /// Stops the connection health monitor thread.
        /// </summary>
        public void StopThread()
        {
            updaterCancellationTokenSource.Cancel();
        }

        private void PollConnectionStatus(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Get connection status
                var connectionStatus = Manager.Tunnel.ConnectionStatus();

                // Update connection bytes sent/received
                UpdateRxTxUI(connectionStatus.RxBytes, connectionStatus.TxBytes);

                // Update connection handshake
                UpdateLastHandshakeUI(connectionStatus.LastHandshakeTimeSec);

                // Update connection stability
                UpdateConnectionStabilityUI(connectionStatus.ConnectionStability);

                // Update connection state
                UpdateConnectionStateUI(connectionStatus.Status);

                // Update tray
                UpdateTrayUI(connectionStatus.Status, connectionStatus.ConnectionStability);

                // Update connection timer
                UpdateConnectionTimer();

                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }

        private void UpdateConnectionStateUI(Models.ConnectionState newStatus)
        {
            var previousStatus = viewModel.Status;

            if (previousStatus != newStatus)
            {
                viewModel.Status = newStatus;

                // Also make sure to not pop up a "disconnected" notification if the initial connection fails
                if (
                    (newStatus == Models.ConnectionState.Protected || newStatus == Models.ConnectionState.Unprotected) &&
                    !(previousStatus == Models.ConnectionState.Connecting && newStatus == Models.ConnectionState.Unprotected)
                )
                {
                    if (newStatus == Models.ConnectionState.Protected)
                    {
                        Manager.TrayIcon.ShowNotification(Manager.TranslationService.GetString("windows-notification-vpn-on-title"), Manager.TranslationService.GetString("windows-notification-vpn-on-content"), NotificationArea.ToastIconType.Connected);
                    }
                }

                if (newStatus == Models.ConnectionState.Unprotected)
                {
                    viewModel.TunnelStatus = Models.ConnectionState.Unprotected;
                    Manager.TrayIcon.ShowNotification(Manager.TranslationService.GetString("windows-notification-vpn-off-title"), Manager.TranslationService.GetString("windows-notification-vpn-off-content"), NotificationArea.ToastIconType.Disconnected);

                    // Force poll account details to ensure that the user's current device hasn't been removed
                    Manager.AccountInfoUpdater.ForcePollAccountInfo();
                }
                else if (newStatus == Models.ConnectionState.Protected)
                {
                    viewModel.TunnelStatus = Models.ConnectionState.Protected;
                }
            }

            UpdateConnectionStateIntegratedUI(newStatus);
        }

        private void UpdateConnectionStateIntegratedUI(Models.ConnectionState newStatus)
        {
            if (newStatus == Models.ConnectionState.Connecting || newStatus == Models.ConnectionState.Disconnecting || Manager.MainWindowViewModel.IsServerSwitching)
            {
                Manager.MainWindowViewModel.IsConnectionTransitioning = true;
            }
            else if (newStatus == Models.ConnectionState.Unprotected || newStatus == Models.ConnectionState.Protected)
            {
                Manager.MainWindowViewModel.IsConnectionTransitioning = false;
            }

            if (Manager.MainWindowViewModel.IsServerSwitching && (newStatus == Models.ConnectionState.Protected || newStatus == Models.ConnectionState.Unprotected))
            {
                // Virtual delay for server switching UI display time
                Task.Delay(TimeSpan.FromSeconds(1.5)).Wait();

                Manager.MainWindowViewModel.IsServerSwitching = false;

                // Show server switch Windows notification
                if (newStatus == Models.ConnectionState.Protected)
                {
                    Manager.TrayIcon.ShowNotification(Manager.TranslationService.GetString("windows-notification-vpn-switch-title", UI.Resources.Localization.TranslationService.Args("currentServer", Manager.MainWindowViewModel.SwitchingServerFrom, new[] { "switchServer", Manager.MainWindowViewModel.SwitchingServerTo })), Manager.TranslationService.GetString("windows-notification-vpn-switch-content"), NotificationArea.ToastIconType.Switched);
                }
            }
        }

        private void UpdateTrayUI(Models.ConnectionState status, Models.ConnectionStability stability)
        {
            if (status != Models.ConnectionState.Protected)
            {
                Manager.TrayIcon.SetDisconnected();
                return;
            }

            // Make sure to try and detect captive portals if unstable/no signal
            if (stability == Models.ConnectionStability.NoSignal || stability == Models.ConnectionStability.Unstable)
            {
                // Attempt to check for a captive portal if the settings option is enabled and captive portal has not already been detected for the current network address
                if (Manager.Settings.Network.CaptivePortalAlert && !Manager.CaptivePortalDetector.CaptivePortalDetected)
                {
                    var ipcDetectCaptivePortalMsg = new IPCMessage(IPCCommand.IpcDetectCaptivePortal);
                    var brokerIPC = Manager.Broker.GetBrokerIPC();
                    brokerIPC.WriteToPipe(ipcDetectCaptivePortalMsg);
                }
            }

            if (stability == Models.ConnectionStability.NoSignal)
            {
                Manager.TrayIcon.SetUnstable();
                return;
            }
            else if (stability == Models.ConnectionStability.Unstable)
            {
                Manager.TrayIcon.SetIdle();
                return;
            }

            Manager.TrayIcon.SetConnected();
        }

        private void UpdateConnectionStabilityUI(Models.ConnectionStability newStability)
        {
            viewModel.Stability = newStability;
        }

        private void UpdateRxTxUI(string newRxBytes, string newTxBytes)
        {
            if (!string.IsNullOrEmpty(newRxBytes) && !string.IsNullOrEmpty(newTxBytes))
            {
                viewModel.RxTx = string.Format("{0}/{1}", ConvertBytesToUserFriendlyString(newRxBytes), ConvertBytesToUserFriendlyString(newTxBytes));
            }
            else
            {
                viewModel.RxTx = string.Empty;
            }
        }

        private void UpdateLastHandshakeUI(string newLastHandshakeTimeSec)
        {
            if (!string.IsNullOrEmpty(newLastHandshakeTimeSec))
            {
                viewModel.LastHandshakeState = string.Format("Last handshake time: {0}", FromUnixTimeString(newLastHandshakeTimeSec).ToLocalTime());
            }
            else
            {
                viewModel.LastHandshakeState = string.Empty;
            }
        }

        private void UpdateConnectionTimer()
        {
            var connectionTime = Manager.Tunnel.GetConnectionTime();
            var time = DateTime.Now - connectionTime;
            var days = string.Empty;

            if (connectionTime == DateTime.MinValue)
            {
                viewModel.ConnectionTime = "00:00:00";
                return;
            }

            if (time.TotalDays >= 1)
            {
                var extraS = time.TotalDays >= 2 ? "s" : string.Empty;
                days = string.Format(string.Concat("{0} day", extraS, " "), Math.Floor(time.TotalDays).ToString());
            }

            viewModel.ConnectionTime = string.Format("{0}{1:00}:{2:00}:{3:00}", days, time.Hours, time.Minutes, time.Seconds);
        }

        private DateTime FromUnixTimeString(string unixTimeString)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(int.Parse(unixTimeString));
        }

        private string ConvertBytesToUserFriendlyString(string bytes)
        {
            string[] units = new string[] { "B", "KiB", "MiB", "GiB", "TiB" };
            string measurementUnit = units[0];
            decimal.TryParse(bytes, out decimal value);

            for (var i = 0; i < units.Length; i++)
            {
                measurementUnit = units[i];

                if (value < 1024)
                {
                    break;
                }

                value /= 1024;
            }

            return string.Format("{0:0.##}{1}", value, measurementUnit);
        }
    }
}
