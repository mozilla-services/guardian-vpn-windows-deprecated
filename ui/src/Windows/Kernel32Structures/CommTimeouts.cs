// <copyright file="CommTimeouts.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Contains the time-out parameters for a communications device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class CommTimeouts
    {
        /// <summary>
        /// Gets or sets the maximum time allowed to elapse before the arrival of the next byte on the communications line, in milliseconds.
        /// </summary>
        public uint ReadIntervalTimeout { get; set; }

        /// <summary>
        /// Gets or sets the multiplier used to calculate the total time-out period for read operations, in milliseconds.
        /// </summary>
        public uint ReadTotalTimeoutMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the constant used to calculate the total time-out period for read operations, in milliseconds. For each read operation, this value is added to the product of the ReadTotalTimeoutMultiplier member and the requested number of bytes.
        /// </summary>
        public uint ReadTotalTimeoutConstant { get; set; }

        /// <summary>
        /// Gets or sets the multiplier used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is multiplied by the number of bytes to be written.
        /// </summary>
        public uint WriteTotalTimeoutMultiplier { get; set; }

        /// <summary>
        /// Gets or sets the constant used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is added to the product of the WriteTotalTimeoutMultiplier member and the number of bytes to be written.
        /// </summary>
        public uint WriteTotalTimeoutConstant { get; set; }
    }
}
