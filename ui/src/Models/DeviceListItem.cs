// <copyright file="DeviceListItem.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateNetwork.Models
{
    /// <summary>
    /// Device list item associated with a FxA user's account.
    /// </summary>
    public class DeviceListItem
    {
        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the device public key.
        /// </summary>
        public string Pubkey { get; set; }

        /// <summary>
        /// Gets or sets the device created date.
        /// </summary>
        public string Created { get; set; }

        /// <summary>
        /// Gets or sets the device added date.
        /// </summary>
        public string Added { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the device is the user's current device.
        /// </summary>
        public bool CurrentDevice { get; set; }
    }
}
