// <copyright file="FxALogin.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using IniParser.Model;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.JSONStructures
{
    /// <summary>
    /// Subscription status enum depicting whether the subscription is active or inactive.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// Subscription is currently active.
        /// </summary>
        Active,

        /// <summary>
        /// Subscription is inactive or has expired.
        /// </summary>
        Inactive,
    }

    /// <summary>
    /// FxALogin object received from FxA.
    /// </summary>
    public class FxALogin
    {
        /// <summary>
        /// Gets or sets the user object.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the user's token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the user's current device public key.
        /// </summary>
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Converts token and public key data to an INI string.
        /// </summary>
        /// <returns>INI string for writing to conf files.</returns>
        public string TokenToString()
        {
            var iniData = new IniData();
            iniData["Token"]["Token"] = Token;
            iniData["Keys"]["PublicKey"] = PublicKey;

            return iniData.ToString();
        }

        /// <summary>
        /// Serializes the User object for writing to file.
        /// </summary>
        /// <returns>Serialized User object.</returns>
        public string UserToJson()
        {
            return JsonConvert.SerializeObject(User, Formatting.Indented);
        }
    }
}
