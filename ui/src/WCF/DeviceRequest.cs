// <copyright file="DeviceRequest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// Device request JSON construct.
    /// </summary>
    [DataContract]
    public class DeviceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceRequest"/> class.
        /// </summary>
        /// <param name="deviceName">User friendly name for the device.</param>
        /// <param name="publicKey">Public key associated with the device.</param>
        public DeviceRequest(string deviceName, string publicKey)
        {
            this.DeviceName = deviceName;
            this.PublicKey = publicKey;
        }

        /// <summary>
        /// Gets or sets the user friendly name for the device.
        /// </summary>
        [DataMember(Name = "deviceName")]
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the public key associated with the device.
        /// </summary>
        [DataMember(Name = "publicKey")]
        public string PublicKey { get; set; }
    }
}
