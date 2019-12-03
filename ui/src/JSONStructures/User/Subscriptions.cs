// <copyright file="Subscriptions.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using IniParser.Model;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// FxA subscriptions.
    /// </summary>
    public class Subscriptions
    {
        /// <summary>
        /// Gets or sets the VPN subscription objectassociated with this FxA account.
        /// </summary>
        [JsonProperty("vpn")]
        public Vpn Vpn { get; set; }
    }
}
