// <copyright file="Cache.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Cached variables used within the application.
    /// </summary>
    internal class Cache
    {
        /// <summary>
        /// Gets or sets the list of the servers that is retreived from the FxA API.
        /// </summary>
        public static FxA.ServerList FxAServerList { get; set; }
    }
}
