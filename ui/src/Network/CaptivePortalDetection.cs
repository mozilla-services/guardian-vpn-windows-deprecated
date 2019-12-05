/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Network
{
    public class CaptivePortalDetection
    {
        public enum ConnectivityStatus {
            NoConnectivity = -1,
            CaptivePortalDetected = 0,
            HaveConnectivity = 1
        }
        [DllImport("tunnel.dll", EntryPoint = "TestConnectivity", CallingConvention = CallingConvention.Cdecl)]
        public static extern ConnectivityStatus TestConnectivity();
    }
}
