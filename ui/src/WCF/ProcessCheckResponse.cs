// <copyright file="ProcessCheckResponse.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Serialization;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// WCF process checker.
    /// </summary>
    [DataContract]
    public class ProcessCheckResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCheckResponse"/> class.
        /// </summary>
        /// <param name="uiProcess">True if the UI process is running.</param>
        /// <param name="brokerProcess">True if the broker process is running.</param>
        /// <param name="tunnelProcess">True if the tunnel process is running.</param>
        /// <param name="commands">Commands attached to the process.</param>
        public ProcessCheckResponse(bool uiProcess, bool brokerProcess, bool tunnelProcess, string commands)
        {
            this.UIProcess = uiProcess;
            this.BrokerProcess = brokerProcess;
            this.TunnelProcess = tunnelProcess;
            this.Commands = commands;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UI process is up and running.
        /// </summary>
        [DataMember]
        public bool UIProcess { get; set; }

        /// <summary>
        /// Gets or sets the command line with which the process is running with.
        /// </summary>
        [DataMember]
        public string Commands { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the broker process is up and running.
        /// </summary>
        [DataMember]
        public bool BrokerProcess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tunnel process is up and running.
        /// </summary>
        [DataMember]
        public bool TunnelProcess { get; set; }
    }
}
