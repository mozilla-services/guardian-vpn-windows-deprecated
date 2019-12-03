// <copyright file="LoginRequest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// LoginRequest WCF command.
    /// </summary>
    [DataContract]
    public class LoginRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRequest"/> class.
        /// </summary>
        /// <param name="verificationUrl">Verification URL to use.</param>
        /// <param name="pollInterval">Poll interval value.</param>
        /// <param name="expiresOn">Date at which the request expires.</param>
        public LoginRequest(string verificationUrl, int pollInterval, DateTime expiresOn)
        {
            this.VerificationUrl = verificationUrl;
            this.PollInterval = pollInterval;
            this.ExpiresOn = expiresOn;
        }

        /// <summary>
        /// Gets or sets the verification URL to poll.
        /// </summary>
        [DataMember(Name = "verification_url")]
        public string VerificationUrl { get; set; }

        /// <summary>
        /// Gets or sets the value indicating how often to poll for login confirmation.
        /// </summary>
        [DataMember(Name = "poll_interval")]
        public int PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the poller.
        /// </summary>
        [DataMember(Name = "expires_on")]
        public DateTime ExpiresOn { get; set; }
    }
}
