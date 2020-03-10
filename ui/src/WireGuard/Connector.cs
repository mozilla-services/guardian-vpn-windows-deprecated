// <copyright file="Connector.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Threading;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Helper class for initiating or terminating VPN connections.
    /// </summary>
    internal class Connector
    {
        /// <summary>
        /// Initiates the connection to a VPN server.
        /// </summary>
        /// <param name="switchServer">True if we're switching servers instead of connecting for the first time.</param>
        /// <param name="previousServerCity">If switching, set to previous server city name for displaying in the UI.</param>
        /// <param name="switchServerCity">If switching, set to the new server city name for displaying in the UI.</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool Connect(bool switchServer = false, string previousServerCity = "", string switchServerCity = "")
        {
            ErrorHandling.DebugLogger.LogDebugMsg("Connect command initiated");
            var configuration = new WireGuard.Config(ProductConstants.FirefoxPrivateNetworkConfFile);

            string address = Manager.Settings.Network.IPv4Address;

            if (Manager.Settings.Network.EnableIPv6)
            {
                address += "," + Manager.Settings.Network.IPv6Address;
            }

            ErrorHandling.DebugLogger.LogDebugMsg("Setting endpoint to", Manager.MainWindowViewModel.ServerSelected.Endpoint);
            var currentServer = FxA.Cache.FxAServerList.GetServerByIP(Manager.MainWindowViewModel.ServerSelected.Endpoint);
            configuration.SetEndpoint(currentServer.GetEndpointWithRandomPort(), currentServer.PublicKey, ProductConstants.AllowedIPs, address, currentServer.DNSServerAddress);

            if (switchServer && Manager.MainWindowViewModel.Status == Models.ConnectionState.Protected)
            {
                // Set "switching" bindings so they show up in the UI
                Manager.MainWindowViewModel.IsServerSwitching = true;
                Manager.MainWindowViewModel.SwitchingServerFrom = previousServerCity;
                Manager.MainWindowViewModel.SwitchingServerTo = switchServerCity;
                Manager.ConnectionStatusUpdater.StartConnectionTransitionStopwatch();

                new Thread(() =>
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Switching endpoint to ", currentServer.Name, currentServer.IPv4Address);
                    Manager.Tunnel.SwitchServer(configuration.Peer.Endpoint, configuration.Peer.PublicKey);
                    Network.Pinger.Ping(currentServer.DNSServerAddress);
                }).Start();
                return true;
            }
            else
            {
                ErrorHandling.DebugLogger.LogDebugMsg("Connecting to ", currentServer.Name, currentServer.IPv4Address);
                return Manager.Tunnel.Connect();
            }
        }

        /// <summary>
        /// Terminate the connection from a VPN server.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        public static bool Disconnect()
        {
            ErrorHandling.DebugLogger.LogDebugMsg("Disconnect initiated");
            return Manager.Tunnel.Disconnect();
        }

        /// <summary>
        /// Parse WireGuard exit codes for display inside the UI.
        /// </summary>
        /// <param name="errorCode">WireGuard error code to parse.</param>
        public static void HandleTunnelFailure(WireGuardTunnelExitCodes errorCode)
        {
            switch (errorCode)
            {
                case WireGuardTunnelExitCodes.ErrorCreateWintun:
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-driver-missing-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    break;

                case WireGuardTunnelExitCodes.ErrorFirewall:
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-firewall-rules-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    break;

                case WireGuardTunnelExitCodes.ErrorRingloggerOpen:
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-open-log-file-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    break;

                case WireGuardTunnelExitCodes.ErrorLoadConfiguration:
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-load-configuration-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    break;

                default:
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-vpn-start-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    break;
            }
        }
    }
}
