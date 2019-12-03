// <copyright file="VersionRequest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Serialization;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// Version request object.
    /// </summary>
    [DataContract]
    public class VersionRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionRequest"/> class.
        /// </summary>
        /// <param name="currentVersion">The current version string to use.</param>
        public VersionRequest(string currentVersion)
        {
            this.CurrentVersion = currentVersion;
        }

        /// <summary>
        /// Gets or sets the crrent version being used.
        /// </summary>
        [DataMember]
        public string CurrentVersion { get; set; }
    }
}
