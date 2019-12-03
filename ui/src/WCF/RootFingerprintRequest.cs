// <copyright file="RootFingerprintRequest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Serialization;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// Root fingerprint object.
    /// </summary>
    [DataContract]
    public class RootFingerprintRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootFingerprintRequest"/> class.
        /// </summary>
        /// <param name="rootFingerprint">The root Fingerprint.</param>
        public RootFingerprintRequest(string rootFingerprint)
        {
            this.RootFingerprint = rootFingerprint;
        }

        /// <summary>
        /// Gets or sets the root fingerprint.
        /// </summary>
        [DataMember]
        public string RootFingerprint { get; set; }
    }
}
