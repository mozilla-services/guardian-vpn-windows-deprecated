// <copyright file="ServiceDescription.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Service description text packaged up to be used as an "lpInfo" parameter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 8192)]
    [ComVisible(false)]
    public struct ServiceDescription
    {
        /// <summary>
        /// Service description.
        /// </summary>
        public string Description;
    }
}
