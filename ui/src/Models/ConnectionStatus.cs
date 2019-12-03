// <copyright file="ConnectionStatus.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateNetwork.Models
{
    /// <summary>
    /// Connection state descriptor.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Tunnel is down, connection is inactive.
        /// </summary>
        Unprotected = 0,

        /// <summary>
        /// Tunnel is up, connection is active.
        /// </summary>
        Protected = 1,

        /// <summary>
        /// Tunnel is in the process of connecting and will go up in a few moments.
        /// </summary>
        Connecting = 2,

        /// <summary>
        /// Disconnection is in progress and the tunnel will go down in a few moments.
        /// </summary>
        Disconnecting = 3,
    }

    /// <summary>
    /// Connection stability descriptor.
    /// </summary>
    public enum ConnectionStability
    {
        /// <summary>
        /// Connection is stable.
        /// </summary>
        Stable = 0,

        /// <summary>
        /// Connection is unstable. No data has been received for a while.
        /// </summary>
        Unstable = 1,

        /// <summary>
        /// No keepalive packets have been received for a while.
        /// </summary>
        NoSignal = 2,
    }

    /// <summary>
    /// Represents the VPN connection status.
    /// </summary>
    public class ConnectionStatus
    {
        /// <summary>
        /// Gets or sets the VPN connection state.
        /// </summary>
        public ConnectionState Status { get; set; }

        /// <summary>
        /// Gets or sets the VPN connection stability.
        /// </summary>
        public ConnectionStability ConnectionStability { get; set; }

        /// <summary>
        /// Gets or sets the private key of the interface used to establish the VPN connection.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the listening port of the interface.
        /// </summary>
        public string ListenPort { get; set; }

        /// <summary>
        /// Gets or sets the hex-encoded public key of peer.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the hex-encoded preshared key of the previously added peer entry.
        /// </summary>
        public string PresharedKey { get; set; }

        /// <summary>
        /// Gets or sets the corresponding peer's protocol version.
        /// </summary>
        public string ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the IP:port for IPv4 or [IP]:port for IPV6, indicating the endpoint of the previously added peer entry.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds since the most recent handshake previously added peer entry, expressed relative to the Unix epoch.
        /// </summary>
        public string LastHandshakeTimeSec { get; set; }

        /// <summary>
        /// Gets or sets the number of nano-seconds since the most recent handshake previously added peer entry, expressed relative to the Unix epoch.
        /// </summary>
        public string LastHandshakeTimeNsec { get; set; }

        /// <summary>
        /// Gets or sets the number of transmitted bytes for the previously added peer entry.
        /// </summary>
        public string TxBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of received bytes for the previously added peer entry.
        /// </summary>
        public string RxBytes { get; set; }

        /// <summary>
        /// Gets or sets the persistent keep alive interval of previously added peer entry. The value 0 disables it.
        /// </summary>
        public string PersistenKeepaliveInterval { get; set; }

        /// <summary>
        /// Gets or sets IP/cidr, indicating allowed IP entries for the previously added peer entry.
        /// </summary>
        public string AllowedIp { get; set; }

        /// <summary>
        /// Gets or sets the error number of the tunnel named pipe get command.
        /// </summary>
        public string ErrNo { get; set; }
    }
}