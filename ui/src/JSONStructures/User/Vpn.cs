// <copyright file="Vpn.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using IniParser.Model;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// VPN subscription info object.
    /// </summary>
    public class Vpn
    {
        /// <summary>
        /// Gets or sets a value indicating whether the VPN subscription is active or not.
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the VPN subscription was started.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the VPN subscription renews.
        /// </summary>
        [JsonProperty("renews_on")]
        public DateTime RenewsOn { get; set; }
    }
}
