// <copyright file="WlanWatcher.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.Network
{
    /// <summary>
    /// Detects connections to unsecure WiFi networks.
    /// </summary>
    public class WlanWatcher : IDisposable
    {
        private readonly Dictionary<string, AccessPoint> accessPoints;
        private WlanNotificationCallback wlanNotifyDelegate;
        private IntPtr clientHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="WlanWatcher"/> class. Starts listening for connections to open WLAN networks.
        /// </summary>
        public WlanWatcher()
        {
            accessPoints = new Dictionary<string, AccessPoint>();
            StartListening();
        }

        /// <summary>
        /// Notification callback delegate. When WLAN properties change, we can tell Windows to ping us.
        /// </summary>
        /// <param name="notificationData">WLAN notification data structure containing details about the notification.</param>
        /// <param name="context">Notification context, not used.</param>
        public delegate void WlanNotificationCallback(ref WlanApi.WlanNotificationData notificationData, IntPtr context);

        /// <summary>
        /// Registers a function to be called whenever a change occurs in a local WLAN connection.
        /// </summary>
        /// <param name="hClientHandle">The client's session handle, obtained by a previous call to the WlanOpenHandle function.</param>
        /// <param name="dwNotifSource">The notification sources to be registered.</param>
        /// <param name="bIgnoreDuplicate">Specifies whether duplicate notifications will be ignored.</param>
        /// <param name="funcCallback">Defines the type of notification callback function.</param>
        /// <param name="pCallbackContext">A pointer to the client context that will be passed to the callback function with the notification.</param>
        /// <param name="pReserved">Reserved for future use. Must be set to IntPtr.Zero.</param>
        /// <param name="pdwPrevNotifSource">A pointer to the previously registered notification sources.</param>
        /// <returns>If successful, the return value is ERROR_SUCCESS (0).</returns>
        [DllImport("Wlanapi.dll", EntryPoint = "WlanRegisterNotification")]
        public static extern uint WlanRegisterNotification(IntPtr hClientHandle, WlanApi.WlanNotificationSource dwNotifSource, bool bIgnoreDuplicate, WlanNotificationCallback funcCallback, IntPtr pCallbackContext, IntPtr pReserved, [Out] out WlanApi.WlanNotificationSource pdwPrevNotifSource);

        /// <summary>
        /// Callback function for reacting to changes in wlan connectivity.
        /// </summary>
        /// <param name="notificationData">WLAN notification data structure containing details about the notification.</param>
        /// <param name="context">Notification context, not used.</param>
        public void WlanNotifyHook(ref WlanApi.WlanNotificationData notificationData, IntPtr context)
        {
            // Do not perform wlan detection if user's unsecure network alert setting is turned off.
            if (!Manager.Settings.Network.UnsecureNetworkAlert)
            {
                return;
            }

            // If connected, ignore any and all messages
            if (Manager.MainWindowViewModel.Status == Models.ConnectionState.Protected)
            {
                return;
            }

            const int clientVersion = 2;
            if ((WlanApi.WlanNotificationMsm)notificationData.NotificationCode == WlanApi.WlanNotificationMsm.Connected)
            {
                if (WlanApi.WlanOpenHandle(clientVersion, IntPtr.Zero, out uint _, out IntPtr clientHandle) != 0)
                {
                    ErrorHandling.ErrorHandler.Handle("Can't open Wlan handle for unsecured network detection.", ErrorHandling.LogLevel.Error);
                    return;
                }

                var queryData = IntPtr.Zero;
                var wlanQueryResult = WlanApi.WlanQueryInterface(clientHandle, new Guid(notificationData.InterfaceGuid.ToString()), WlanApi.WlanIntfOpcode.CurrentConnection, IntPtr.Zero, out _, ref queryData, IntPtr.Zero);

                if (wlanQueryResult == 0)
                {
                    var connectionData = Marshal.PtrToStructure<WlanApi.WlanConnectionAttributes>(queryData);

                    // Detect open network or WEP secured network
                    if (connectionData.WlanSecurityAttributes.Dot11AuthAlgorithm == WlanApi.Dot11AuthAlgorithm.Open ||
                        (
                         connectionData.WlanSecurityAttributes.Dot11CipherAlgorithm == WlanApi.Dot11CipherAlgorithm.Wep ||
                         connectionData.WlanSecurityAttributes.Dot11CipherAlgorithm == WlanApi.Dot11CipherAlgorithm.Wep40 ||
                         connectionData.WlanSecurityAttributes.Dot11CipherAlgorithm == WlanApi.Dot11CipherAlgorithm.Wep104)
                        )
                    {
                        var showNotification = true;
                        var bSsid = connectionData.WlanAssociationAttributes.Dot11Bssid.ToString();
                        if (!accessPoints.ContainsKey(connectionData.WlanAssociationAttributes.Dot11Bssid.ToString()))
                        {
                            var newAp = new AccessPoint()
                            {
                                Ssid = connectionData.ProfileName,
                                Bssid = bSsid,
                                LastNotified = DateTime.UtcNow,
                            };
                            accessPoints[bSsid] = newAp;
                        }
                        else
                        {
                            if ((DateTime.UtcNow - accessPoints[bSsid].LastNotified).TotalSeconds < ProductConstants.InsecureWiFiTimeout)
                            {
                                showNotification = false;
                            }

                            var accessPoint = accessPoints[bSsid];
                            accessPoint.LastNotified = DateTime.UtcNow;
                            accessPoints[bSsid] = accessPoint;
                        }

                        if (showNotification)
                        {
                            Manager.TrayIcon.ShowNotification(
                                Manager.TranslationService.GetString("windows-notification-unsecure-network-title"),
                                Manager.TranslationService.GetString("windows-notification-unsecure-network-content", UI.Resources.Localization.TranslationService.Args("wifiName", connectionData.WlanAssociationAttributes.Dot11Ssid.ToString())),
                                NotificationArea.ToastIconType.Disconnected
                            );
                        }
                    }

                    if (queryData != IntPtr.Zero)
                    {
                        WlanApi.WlanFreeMemory(queryData);
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the WLAN handle.
        /// </summary>
        public void Dispose()
        {
            StopListening();
        }

        private bool StartListening()
        {
            try
            {
                var result = WlanApi.WlanOpenHandle(2, IntPtr.Zero, out uint currentVersion, out clientHandle);
                if (result != 0)
                {
                    return false;
                }

                wlanNotifyDelegate = new WlanNotificationCallback(WlanNotifyHook);
                return WlanRegisterNotification(clientHandle, WlanApi.WlanNotificationSource.MSM, true, wlanNotifyDelegate, IntPtr.Zero, IntPtr.Zero, out WlanApi.WlanNotificationSource prevSource) == 0;
            }
            catch (System.DllNotFoundException e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }

            return false;
        }

        private void StopListening()
        {
            WlanApi.WlanCloseHandle(clientHandle, IntPtr.Zero);
        }

        /// <summary>
        /// Accesss point structure containing SSIDs and MACs.
        /// </summary>
        public struct AccessPoint
        {
            /// <summary>
            /// Name of the access point.
            /// </summary>
            public string Ssid;

            /// <summary>
            /// MAC address of the access point.
            /// </summary>
            public string Bssid;

            /// <summary>
            /// Last time the user was notified about this AP.
            /// </summary>
            public DateTime LastNotified;
        }
    }
}