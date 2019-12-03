// <copyright file="Country.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// Country object containing information about a country, along with cities and VPN servers within it.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the version number for the minimum supported client.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version number for the minimum supported client.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the version number for the minimum supported client.
        /// </summary>
        [JsonProperty("cities")]
        public List<City> Cities { get; set; }
    }
}
