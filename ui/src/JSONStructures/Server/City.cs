// <copyright file="City.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// City object containing information about a city, along with VPN servers within it.
    /// </summary>
    public class City
    {
        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short code to be used when addressing this city.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the longitude in this city.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of this city.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the servers in this city.
        /// </summary>
        [JsonProperty("servers")]
        public List<Server> Servers { get; set; }
    }
}
