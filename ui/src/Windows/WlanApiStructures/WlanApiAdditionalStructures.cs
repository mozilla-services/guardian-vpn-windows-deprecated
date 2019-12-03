// <copyright file="WlanApiAdditionalStructures.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

/* SPDX-License-Identifier: MIT
 *
 * ManagedNativeWifi
 * Copyright (c) 2015-2019 emoacht
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Windows.WlanApiStructures
{
    /// <summary>
    /// Additional WlanApi structures with conversion logic built in.
    /// </summary>
    public class WlanApiAdditionalStructures
    {
        /// <summary>
        /// Used to define an IEEE media access control (MAC) address.
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-mac-address-type" />.
        [StructLayout(LayoutKind.Sequential)]
        public struct Dot11MacAddress
        {
            /// <summary>
            /// MAC address.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MacAddress;

            /// <summary>
            /// Returns a MAC address in string form.
            /// </summary>
            /// <returns>MAC address.</returns>
            public override string ToString()
            {
                return (MacAddress != null)
                    ? BitConverter.ToString(MacAddress).Replace('-', ':')
                    : null;
            }
        }

        /// <summary>
        /// Contains the SSID of an interface.
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/nativewifi/dot11-ssid" />.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Dot11Ssid
        {
            /// <summary>
            /// The length, in bytes, of the Ssid array.
            /// </summary>
            public uint SsidLength;

            /// <summary>
            /// The SSID. Max length is 32.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Ssid;

            /// <summary>
            /// Type of encoding. Default: utf-8 (65001).
            /// </summary>
            private static readonly Encoding Encoding = Encoding.GetEncoding(65001, EncoderFallback.ReplacementFallback, DecoderFallback.ExceptionFallback);

            /// <summary>
            /// Converts the SSID to a byte array.
            /// </summary>
            /// <returns>Byte array containing SSID.</returns>
            public byte[] ToBytes() => Ssid?.Take((int)SsidLength).ToArray();

            /// <summary>
            /// Retrieves the SSID in string form.
            /// </summary>
            /// <returns>SSID.</returns>
            public override string ToString()
            {
                if (Ssid == null)
                {
                    return null;
                }

                try
                {
                    return Encoding.GetString(ToBytes());
                }
                catch (DecoderFallbackException)
                {
                    return null;
                }
            }
        }
    }
}
