// <copyright file="BalrogResponse.cs" company="Mozilla">
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
    /// Balrog JSON response structure.
    /// </summary>
    public class BalrogResponse
    {
        /// <summary>
        /// Gets or sets the latest version of the application.
        /// </summary>
        [JsonProperty("version")]
        public string LatestVersion { get; set; }

        /// <summary>
        /// Gets or sets the URL of the latest MSI.
        /// </summary>
        [JsonProperty("url")]
        public string MsiUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this update is mandatory.
        /// </summary>
        [JsonProperty("required")]
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the used hash function to verify the MSI.
        /// </summary>
        [JsonProperty("hashFunction")]
        public string HashFunction { get; set; }

        /// <summary>
        /// Gets or sets the checksum of the MSI file.
        /// </summary>
        [JsonProperty("hashValue")]
        public string HashValue { get; set; }
    }
}
