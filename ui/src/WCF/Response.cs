// <copyright file="Response.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Serialization;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// WCF response.
    /// </summary>
    [DataContract]
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="status">Status code.</param>
        /// <param name="message">Response message contents.</param>
        /// <param name="stackTrace">Stack trace contents.</param>
        public Response(int status, string message, string stackTrace = null)
        {
            this.Status = status;
            this.Message = message;
            this.StackTrace = stackTrace;
        }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace contents.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }
    }
}
