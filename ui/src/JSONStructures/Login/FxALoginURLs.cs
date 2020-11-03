// <copyright file="FxALoginURLs.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// URLs received from FxA to proceed with the login flow.
    /// </summary>
    public class FxALoginURLs
    {
        /// <summary>
        /// Gets or sets the login URL to pop up.
        /// </summary>
        [JsonProperty("login_url")]
        public string LoginUrl { get; set; }

        /// <summary>
        /// Gets or sets the verification URL to poll.
        /// </summary>
        [JsonProperty("verification_url")]
        public string VerificationUrl { get; set; }

        /// <summary>
        /// Gets or sets the expiration date/time for the URLs.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTime ExpiresOn { get; set; }

        /// <summary>
        /// Gets or sets the recommended poll interval for the verification URL.
        /// </summary>
        [JsonProperty("poll_interval")]
        public int PollInterval { get; set; }
    }
}
