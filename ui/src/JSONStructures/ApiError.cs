// <copyright file="ApiError.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// ApiError object structure received from FxA.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the ApiError code.
        /// </summary>
        [JsonProperty("code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error number of the ApiError.
        /// </summary>
        [JsonProperty("errno")]
        public int ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("HTTP {0} [{1}] {2}", ErrorCode, ErrorNumber, Error);
        }
    }
}
