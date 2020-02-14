// <copyright file="Config.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Manages the WireGuard service configuration file.
    /// </summary>
    internal class Config
    {
        private readonly string filePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="privateKey">Private key of the interface.</param>
        /// <param name="address">Address of the interface.</param>
        /// <param name="dns">DNS of the interface.</param>
        public Config(string privateKey, string address, string dns)
        {
            Interface = new InterfaceProperties()
            {
                PrivateKey = privateKey,
                Address = address,
                DNS = dns,
            };

            Peer = new PeerProperties()
            {
                PublicKey = string.Empty,
                AllowedIPs = string.Empty,
                Endpoint = string.Empty,
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="filePath">Name of WireGuard service configuration file.</param>
        public Config(string filePath)
        {
            this.filePath = filePath;
            ReadFromFile(filePath);
        }

        /// <summary>
        /// Gets or sets the WireGuard client interface configuration properties.
        /// </summary>
        [DisplayName("Interface")]
        public InterfaceProperties Interface { get; set; }

        /// <summary>
        /// Gets or sets the WireGuard peer configuration properties.
        /// </summary>
        [DisplayName("Peer")]
        public PeerProperties Peer { get; set; }

        /// <summary>
        /// Gets the peer endpoint address without the port number.
        /// </summary>
        /// <returns>The peer endpoint address without the port number.</returns>
        public string GetPeerEndpointWithoutPort()
        {
            var currentEndpointWithoutPort = string.Empty;

            if (!string.IsNullOrEmpty(Peer.Endpoint))
            {
                currentEndpointWithoutPort = Peer.Endpoint.Split(':')[0];
            }

            return currentEndpointWithoutPort;
        }

        /// <summary>
        /// Sets the endpoint in a conf file, as well as a public key.
        /// </summary>
        /// <param name="endpoint">The endpoint of the peer.</param>
        /// <param name="publicKey">The public key of the peer.</param>
        /// <param name="allowedIPs">A space-delimited string of the allowed IPs for the peer.</param>
        /// <param name="address"> The IP address for the Interface. </param>
        /// <param name="dns">The DNS server address of the interface.</param>
        public void SetEndpoint(string endpoint, string publicKey, string allowedIPs, string address, string dns)
        {
            var newPeer = Peer;
            newPeer.Endpoint = endpoint;
            newPeer.AllowedIPs = allowedIPs;
            newPeer.PublicKey = publicKey;
            Peer = newPeer;

            var newInterface = Interface;
            newInterface.Address = address;
            newInterface.DNS = dns;
            Interface = newInterface;

            WriteToFile(filePath);
        }

        /// <summary>
        /// Writes the WireGuard service configuration to file.
        /// </summary>
        /// <param name="filePath">File path of WireGuard service configuration file.</param>
        /// <returns>Success status of writing to the WireGuard service configuration file.</returns>
        public bool WriteToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, this.ToString());

                // Set access control for file in order to have access to the WireGuard named pipe
                var accessControl = File.GetAccessControl(filePath);
                var ntAccount = new NTAccount(Environment.UserDomainName, Environment.UserName);
                accessControl.SetOwner(ntAccount);
                File.SetAccessControl(filePath, accessControl);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the WireGuard service configuration from file.
        /// </summary>
        /// <param name="filePath">File path of WireGuard service configuration file.</param>
        /// <returns>Success status of reading from the WireGuard service configuration file.</returns>
        public bool ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            var parser = new IniParser.FileIniDataParser();
            var iniData = parser.ReadFile(filePath);

            Interface = new InterfaceProperties()
            {
                PrivateKey = iniData["Interface"]["PrivateKey"],
                Address = iniData["Interface"]["Address"],
                DNS = iniData["Interface"]["DNS"],
            };

            Peer = new PeerProperties()
            {
                PublicKey = iniData["Peer"]["PublicKey"],
                AllowedIPs = iniData["Peer"]["AllowedIPs"],
                Endpoint = iniData["Peer"]["Endpoint"],
            };

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var returnString = new StringBuilder();
            var sectionProperties = typeof(Config).GetProperties();

            foreach (PropertyInfo sectionProperty in sectionProperties)
            {
                var sectionDisplayName = sectionProperty.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single().DisplayName;
                returnString.Append(string.Format("[{0}]\n", sectionDisplayName));

                var infoProperties = sectionProperty.PropertyType.GetProperties();
                var section = this.GetType().GetProperty(sectionProperty.Name).GetValue(this, null);

                if (section == null)
                {
                    continue;
                }

                foreach (PropertyInfo infoProperty in infoProperties)
                {
                    var infoDisplayName = infoProperty.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single().DisplayName;
                    string infoValue = section.GetType().GetProperty(infoProperty.Name).GetValue(section, null).ToString();
                    if (infoValue == null)
                    {
                        continue;
                    }

                    returnString.Append(string.Format("{0} = {1}\n", infoDisplayName, infoValue));
                }

                returnString.Append("\n");
            }

            return returnString.ToString();
        }

        /// <summary>
        /// Represents the configuration properties pertaining to the interface.
        /// </summary>
        public struct InterfaceProperties
        {
            /// <summary>
            /// Gets or sets the private key of the interface.
            /// </summary>
            [DisplayName("PrivateKey")]
            public string PrivateKey { get; set; }

            /// <summary>
            /// Gets or sets the address of the interface.
            /// </summary>
            [DisplayName("Address")]
            public string Address { get; set; }

            /// <summary>
            /// Gets or sets the DNS of the interface.
            /// </summary>
            [DisplayName("DNS")]
            public string DNS { get; set; }
        }

        /// <summary>
        /// Represents the configuration properties pertaining to the peer.
        /// </summary>
        public struct PeerProperties
        {
            /// <summary>
            /// Gets or sets the public key of the peer.
            /// </summary>
            [DisplayName("PublicKey")]
            public string PublicKey { get; set; }

            /// <summary>
            /// Gets or sets the allowed IPs of the peer.
            /// </summary>
            [DisplayName("AllowedIPs")]
            public string AllowedIPs { get; set; }

            /// <summary>
            /// Gets or sets the endpoint of the peer.
            /// </summary>
            [DisplayName("Endpoint")]
            public string Endpoint { get; set; }
        }
    }
}
