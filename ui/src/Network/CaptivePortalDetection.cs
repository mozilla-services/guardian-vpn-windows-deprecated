// <copyright file="CaptivePortalDetection.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Network
{
    /// <summary>
    /// Used for detecting connectivity issues due to captive portals.
    /// </summary>
    public class CaptivePortalDetection
    {
        private bool captivePortalDetected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptivePortalDetection"/> class.
        /// </summary>
        public CaptivePortalDetection()
        {
            // Add an event handler to reset the captive portal detection check when a network change is detected
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler((sender, e) =>
            {
                Task.Delay(System.TimeSpan.FromSeconds(5)).ContinueWith(task => { CaptivePortalDetected = false; });
            });
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
                    // Send a windows notification if captive portal is detected.
                    Manager.TrayIcon.ShowNotification(
                        Manager.TranslationService.GetString("windows-notification-captive-portal-title"),
                        Manager.TranslationService.GetString("windows-notification-captive-portal-content"),
                        NotificationArea.ToastIconType.Disconnected
                    );
                }
            }
        }

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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation which returns a ConnectivityStatus value.</returns>
        public static Task<ConnectivityStatus> IsCaptivePortalActiveTask()
        {
            var testOutsideConnectivityTask = Task.Run(() =>
            {
                return TestOutsideConnectivity(ProductConstants.CaptivePortalDetectionIP, ProductConstants.CaptivePortalDetectionHost, ProductConstants.CaptivePortalDetectionUrl, ProductConstants.CaptivePortalDetectionValidReplyContents);
            });

            return testOutsideConnectivityTask;
        }
    }
}
