// <copyright file="ServiceSidInfo.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Represents a service security identifier (SID).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192)]
    [ComVisible(false)]
    public struct ServiceSidInfo
    {
        /// <summary>
        /// The service SID type.
        /// </summary>
        public ServiceSidType ServiceSidType;
    }
}
