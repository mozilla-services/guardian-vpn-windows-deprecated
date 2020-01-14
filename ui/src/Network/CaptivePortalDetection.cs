// <copyright file="CaptivePortalDetection.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FirefoxPrivateNetwork.NotificationArea;
using Microsoft.WindowsAPICodePack.Net;

namespace FirefoxPrivateNetwork.Network
{
    /// <summary>
    /// Used for detecting connectivity issues due to captive portals.
    /// </summary>
    public class CaptivePortalDetection
    {
        private readonly TimeSpan postLoginNotificationGracePeriod = TimeSpan.FromSeconds(10);
        private readonly TimeSpan monitorInternetConnectivityFrequency = TimeSpan.FromSeconds(10);
        private readonly TimeSpan monitorInternetConnectivityTimeout = TimeSpan.FromMinutes(15);

        private Task resolveCaptivePortalDetectionHostTask;
        private bool captivePortalDetected = false;
        private List<Microsoft.WindowsAPICodePack.Net.Network> connectedNetworks = new List<Microsoft.WindowsAPICodePack.Net.Network>();
        private CancellationTokenSource monitorInternetConnectivityTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptivePortalDetection"/> class.
        /// </summary>
        public CaptivePortalDetection()
        {
            // Add an event handler to reset the captive portal detection check when a network change is detected
            ConfigureNetworkAddressChangedHandler();

            // Attempt to resolve the captive portal detection host to ip
            ResolveCaptivePortalDetectionHost();
        }

        /// <summary>
        /// Connectivity status result when checking for connectivity/captive portal presense.
        /// </summary>
        public enum ConnectivityStatus
        {
            /// <summary>
            /// There is no internet connectivity available at this time.
            /// </summary>
            NoConnectivity = -1,

            /// <summary>
            /// Potential internet connectivity, as a captive portal has been detected.
            /// </summary>
            CaptivePortalDetected,

            /// <summary>
            /// There is internet connectivity available.
            /// </summary>
            HaveConnectivity,
        }

        /// <summary>
        /// Gets or sets a value indicating whether the captive portal host has been resolved or not.
        /// </summary>
        public bool CaptivePortalHostResolved { get; set; } = false;

        /// <summary>
        /// Gets or sets the captive portal detection ip.
        /// </summary>
        public string CaptivePortalDetectionIp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we have detected a captive portal network.
        /// </summary>
        public bool CaptivePortalDetected
        {
            get
            {
                return captivePortalDetected;
            }

            set
            {
                captivePortalDetected = value;
                if (value)
                {
                    CaptivePortalLoggedIn = false;

                    // Send a windows notification if captive portal is detected.
                    Manager.TrayIcon.ShowNotification(
                        Manager.TranslationService.GetString("windows-notification-captive-portal-blocked-title"),
                        Manager.TranslationService.GetString("windows-notification-captive-portal-blocked-content"),
                        NotificationArea.ToastIconType.Disconnected,
                        clickEvent: ToastClickEvent.Disconnect
                    );
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user has logged in to the detected captive portal or not.
        /// </summary>
        public bool CaptivePortalLoggedIn { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether internet connection is being monitored or not.
        /// </summary>
        public bool MonitoringInternetConnection { get; set; } = false;

        /// <summary>
        /// Adds a route to specified IP and tries to retrieve a URL from a host, then checks downloaded contents of file with expectedTestResult.
        /// This will check whether there is any connectivity outside of the confines of a potentially active WireGuard tunnel.
        /// </summary>
        /// <param name="ip">IP address to add to routing table (to route traffic outside of the tunnel).</param>
        /// <param name="host">Host to contact for captive portal detection.</param>
        /// <param name="url">URL to download.</param>
        /// <param name="expectedTestResult">Expected contents of the downloaded file (e.g. "success").</param>
        /// <returns>Returns connectivity status indicating current connection state.</returns>
        [DllImport("tunnel.dll", EntryPoint = "TestOutsideConnectivity", CallingConvention = CallingConvention.Cdecl)]
        public static extern ConnectivityStatus TestOutsideConnectivity([MarshalAs(UnmanagedType.LPWStr)] string ip, [MarshalAs(UnmanagedType.LPWStr)] string host, [MarshalAs(UnmanagedType.LPWStr)] string url, [MarshalAs(UnmanagedType.LPWStr)] string expectedTestResult);

        /// <summary>
        /// Checks whether we are located on a captive portal network.
        /// </summary>
        /// <param name="ip">Captive portal detection ip.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation which returns a ConnectivityStatus value.</returns>
        public static Task<ConnectivityStatus> IsCaptivePortalActiveTask(string ip)
        {
            var testOutsideConnectivityTask = Task.Run(() =>
            {
                return TestOutsideConnectivity(ip, ProductConstants.CaptivePortalDetectionHost, ProductConstants.CaptivePortalDetectionUrl, ProductConstants.CaptivePortalDetectionValidReplyContents);
            });

            return testOutsideConnectivityTask;
        }

        /// <summary>
        /// Initiates a task that resolves the captive portal detection host.
        /// </summary>
        public void ResolveCaptivePortalDetectionHost()
        {
            // Do not initiate the task if host has already been resolved or the task is already running
            if (CaptivePortalHostResolved || (resolveCaptivePortalDetectionHostTask != null && resolveCaptivePortalDetectionHostTask.Status == TaskStatus.Running))
            {
                return;
            }

            resolveCaptivePortalDetectionHostTask = Task.Run(() =>
            {
                string resolvedIp;

                try
                {
                    // Attempt to resolve the captive portal detection host to ip
                    resolvedIp = Dns.GetHostEntry(ProductConstants.CaptivePortalDetectionHost).AddressList.First(addr => addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                }
                catch (Exception)
                {
                    return;
                }

                // Validate the SSL certificate associated with the resolved ip
                if (!ValidateCaptivePortalDetectionUrlCertificate(resolvedIp))
                {
                    return;
                }

                // Save the resolved ip to the settings conf file
                var networkSettings = Manager.Settings.Network;
                networkSettings.CaptivePortalDetectionIp = resolvedIp;
                Manager.Settings.Network = networkSettings;

                // Set the captive portal host resolved flag
                CaptivePortalHostResolved = true;
            });
        }

        /// <summary>
        /// Initiates a task that monitors the current captive portal network for internet connectivity.
        /// </summary>
        public async void MonitorInternetConnectivity()
        {
            // Initiatilize a new task token source
            monitorInternetConnectivityTokenSource = new CancellationTokenSource();

            var monitorInternetConnectivityTask = Task.Run(() =>
            {
                MonitoringInternetConnection = true;

                while (!monitorInternetConnectivityTokenSource.Token.IsCancellationRequested)
                {
                    if (CheckInternetConnectivity())
                    {
                        // Task delay for the post captive portal login grace period
                        monitorInternetConnectivityTokenSource.Token.WaitHandle.WaitOne(postLoginNotificationGracePeriod);

                        // If the vpn is still turned off, send notification prompting the user to turn it on
                        if (Manager.MainWindowViewModel.Status == Models.ConnectionState.Unprotected)
                        {
                            Manager.TrayIcon.ShowNotification(
                                Manager.TranslationService.GetString("windows-notification-captive-portal-detected-title"),
                                Manager.TranslationService.GetString("windows-notification-captive-portal-detected-content", UI.Resources.Localization.TranslationService.Args("wifiName", connectedNetworks.First().Name)),
                                NotificationArea.ToastIconType.Disconnected,
                                clickEvent: ToastClickEvent.Connect
                            );
                        }

                        CaptivePortalLoggedIn = true;
                        MonitoringInternetConnection = false;
                        CaptivePortalDetected = false;
                        return;
                    }

                    monitorInternetConnectivityTokenSource.Token.WaitHandle.WaitOne(monitorInternetConnectivityFrequency);
                }
            }, monitorInternetConnectivityTokenSource.Token);

            // Cancel the monitor internet connectivity task if the timeout is reached
            if (await Task.WhenAny(monitorInternetConnectivityTask, Task.Delay(monitorInternetConnectivityTimeout)) != monitorInternetConnectivityTask)
            {
                StopMonitorInternetConnectivity();
            }
        }

        /// <summary>
        /// Cancels the task that monitors the current captive portal network for internet connectivity.
        /// </summary>
        public void StopMonitorInternetConnectivity()
        {
            monitorInternetConnectivityTokenSource.Cancel();
            MonitoringInternetConnection = false;
            CaptivePortalDetected = false;
        }

        private bool CheckInternetConnectivity()
        {
            try
            {
                var uri = ProductConstants.CaptivePortalDetectionUrl.Replace("%s", ProductConstants.CaptivePortalDetectionHost);
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "HEAD";
                request.AllowAutoRedirect = false;

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void ConfigureNetworkAddressChangedHandler()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler((sender, e) =>
            {
                // Enumerate the connected network interfaces
                var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).GetEnumerator();
                var newConnectedNetworks = new List<Microsoft.WindowsAPICodePack.Net.Network>();
                while (networks.MoveNext())
                {
                    // Ignore the tunnel network interface and include everything else
                    if (networks.Current.Name != ProductConstants.InternalAppName)
                    {
                        newConnectedNetworks.Add(networks.Current);
                    }
                }

                if (newConnectedNetworks.Count > 0)
                {
                    // Compare the network ids to confirm if a network change occured
                    var connectedNetworkIds = new HashSet<Guid>(connectedNetworks.Select(n => n.NetworkId).ToList());
                    var newConnectedNetworkIds = new HashSet<Guid>(newConnectedNetworks.Select(n => n.NetworkId).ToList());

                    if (!connectedNetworkIds.SetEquals(newConnectedNetworkIds))
                    {
                        connectedNetworks = newConnectedNetworks;

                        // Validate the set of network ids, filtering out the networks that are obsolete
                        if (ValidateNetworkIds(connectedNetworkIds) && ValidateNetworkIds(newConnectedNetworkIds))
                        {
                            CaptivePortalDetected = false;
                        }
                    }
                }
            });
        }

        private bool ValidateNetworkIds(HashSet<Guid> networkIds)
        {
            foreach (var id in networkIds)
            {
                try
                {
                    NetworkListManager.GetNetwork(id);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateCaptivePortalDetectionUrlCertificate(string resolvedIp)
        {
            // Attempt to access the captive portal detection url with the resolved ip
            var certificateValidationUri = new UriBuilder(Uri.UriSchemeHttps, resolvedIp).Uri;
            HttpWebRequest request = WebRequest.CreateHttp(certificateValidationUri);
            request.Host = ProductConstants.CaptivePortalDetectionHost;

            // Configure the server certificate validation delegate
            request.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

            try
            {
                request.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
