// <copyright file="ServerList.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// ServerList object, containing countries, cities and VPN servers.
    /// </summary>
    public class ServerList
    {
        /// <summary>
        /// Gets or sets the countries object containing countries, cities and VPN servers.
        /// </summary>
        [JsonProperty("countries")]
        public List<Country> Countries { get; set; }
    }
}
