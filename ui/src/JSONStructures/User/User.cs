// <copyright file="User.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using IniParser.Model;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// User object.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's avatar URL.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets the user's name to be displayed.
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user's device list.
        /// </summary>
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }

        /// <summary>
        /// Gets or sets the user's subscriptions list.
        /// </summary>
        [JsonProperty("subscriptions")]
        public Subscriptions Subscriptions { get; set; }

        /// <summary>
        /// Gets or sets the user's maximum allowed device count.
        /// </summary>
        [JsonProperty("max_devices")]
        public int MaxDevices { get; set; }
    }
}
