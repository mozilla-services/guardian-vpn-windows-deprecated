// <copyright file="IpInfo.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// Ip info object.
    /// </summary>
    internal class IpInfo
    {
        /// <summary>
        /// Gets or sets the ApiError code.
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the error number of the ApiError.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        [JsonProperty("subregion")]
        public string Subregion { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }
    }
}
