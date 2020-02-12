// <copyright file="IpInfo.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;

namespace FirefoxPrivateNetwork.Models
{
    /// <summary>
    /// IP information for the client.
    /// </summary>
    public class IpInfo
    {
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the country associated with the IP.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the subregion associated with the IP.
        /// </summary>
        public string Subregion { get; set; }

        /// <summary>
        /// Gets or sets the city associated with the IP.
        /// </summary>
        public string City { get; set; }
    }
}
