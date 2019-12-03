// <copyright file="Device.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using IniParser.Model;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// VPN device properties from FxA.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the name of the user's device.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the public key associated with the device.
        /// </summary>
        [JsonProperty("pubkey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the IPv4 address assigned to this device.
        /// </summary>
        [JsonProperty("ipv4_address")]
        public string IPv4Address { get; set; }

        /// <summary>
        /// Gets or sets the IPv6 address assigned to this device.
        /// </summary>
        [JsonProperty("ipv6_address")]
        public string IPv6Address { get; set; }

        /// <summary>
        /// Gets or sets the date/time the device was created at.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
