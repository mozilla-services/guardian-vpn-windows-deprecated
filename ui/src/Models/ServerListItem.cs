// <copyright file="ServerListItem.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;

namespace FirefoxPrivateNetwork.Models
{
    /// <summary>
    /// Data binding class that represents a listed server in the combobox of the MainWindow.
    /// </summary>
    public class ServerListItem
    {
        /// <summary>
        /// Gets or sets the country where the server is located.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the city where the server is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the server code (e.g. rs1-belgrade).
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the endpoint IP address.
        /// </summary>
        public string Endpoint { get; set; }
    }
}
