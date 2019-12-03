// <copyright file="Server.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// Server object containing VPN server endpoint info.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Gets or sets the server's hostname.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the public facing IPv4 endpoint address of the server.
        /// </summary>
        [JsonProperty("ipv4_addr_in")]
        public string IPv4EndpointAddress { get; set; }

        /// <summary>
        /// Gets or sets the weight of the server, indicating load and preferability when in the process of picking random servers.
        /// </summary>
        [JsonProperty("weight")]
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include this server as part of a country.
        /// </summary>
        [JsonProperty("include_in_country")]
        public bool IncludeInCountry { get; set; }

        /// <summary>
        /// Gets or sets the public key of the server.
        /// </summary>
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the port ranges available with this server.
        /// </summary>
        [JsonProperty("port_ranges")]
        public List<List<int>> PortRanges { get; set; }

        /// <summary>
        /// Gets or sets the IPv4 gateway/DNS server when connecting to this server.
        /// </summary>
        [JsonProperty("ipv4_gateway")]
        public string IPv4Gateway { get; set; }

        /// <summary>
        /// Gets or sets the IPv6 gateway/DNS server when connecting to this server.
        /// </summary>
        [JsonProperty("ipv6_gateway")]
        public string IPv6Gateway { get; set; }
    }
}
