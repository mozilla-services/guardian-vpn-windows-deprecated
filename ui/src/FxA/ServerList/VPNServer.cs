// <copyright file="VPNServer.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Represents a VPN server.
    /// </summary>
    public class VPNServer
    {
        private const int MaxPorts = 65535;
        private const int RandomnessPrecision = 100000;

        /// <summary>
        /// Initializes a new instance of the <see cref="VPNServer"/> class.
        /// </summary>
        public VPNServer()
        {
        }

        /// <summary>
        /// Gets or sets the VPN server name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the VPN server country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the VPN server city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the VPN server IPv4 address.
        /// </summary>
        public string IPv4Address { get; set; }

        /// <summary>
        /// Gets or sets the VPN server IPv6 address.
        /// </summary>
        public string IPv6Address { get; set; }

        /// <summary>
        /// Gets or sets the VPN server endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the VPN server DNS address.
        /// </summary>
        public string DNSServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the VPN server public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the VPN server port range.
        /// </summary>
        public List<List<int>> Ports { get; set; }

        /// <summary>
        /// Gets or sets the VPN server weight (based on server load).
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Gets the VPN server endpoint, with a random port selected from the port range.
        /// </summary>
        /// <returns>Endpoint with a random port.</returns>
        public string GetEndpointWithRandomPort()
        {
            return string.Format("{0}:{1}", Endpoint, PickRandomPort(Ports));
        }

        /// <summary>
        /// Picks a random port from a port range.
        /// </summary>
        /// <param name="portRanges">Port range.</param>
        /// <returns>A random port from the port range.</returns>
        public int PickRandomPort(List<List<int>> portRanges)
        {
            var randomPortsRanges = new Dictionary<string, PortRange>();
            var totalWeightScore = 0;

            foreach (var portRangeList in portRanges)
            {
                var from = portRangeList.First();
                var to = portRangeList.Last();
                var totalPorts = Math.Abs(to - from) + 1;

                if (from > to)
                {
                    var tmpTo = to;
                    to = from;
                    from = tmpTo;
                }

                var portRange = new PortRange()
                {
                    From = from,
                    To = to,
                    Count = totalPorts,
                    Weight = 0,
                };

                totalWeightScore += totalPorts;
                randomPortsRanges.Add(string.Format("{0},{1}", from, to), portRange);
            }

            // Sort out the weights
            foreach (var key in randomPortsRanges.Keys.ToList())
            {
                var rangeWithWeight = randomPortsRanges[key];
                rangeWithWeight.Weight = (uint)Math.Ceiling((double)randomPortsRanges[key].Count / (double)totalWeightScore * RandomnessPrecision);
                randomPortsRanges[key] = rangeWithWeight;
            }

            // Pick one range, based on weight
            byte[] randomBlob = new byte[sizeof(uint)];
            byte[] randomPortBlob = new byte[sizeof(ushort)];
            var rngProvider = new RNGCryptoServiceProvider();
            rngProvider.GetBytes(randomBlob);

            var returnPort = 53;
            var randomNumber = BitConverter.ToUInt32(randomBlob, 0) % (RandomnessPrecision + 1);

            try
            {
                foreach (var key in randomPortsRanges.Keys.ToList())
                {
                    if (randomNumber <= randomPortsRanges[key].Weight)
                    {
                        rngProvider.GetBytes(randomPortBlob);
                        var randomPortNumber = BitConverter.ToUInt16(randomPortBlob, 0);
                        returnPort = (randomPortNumber % (randomPortsRanges[key].To + 1 - randomPortsRanges[key].From)) + randomPortsRanges[key].From;
                        break;
                    }

                    randomNumber -= randomPortsRanges[key].Weight;
                }
            }
            finally
            {
                rngProvider.Dispose();
            }

            return returnPort;
        }

        /// <summary>
        /// Represent's a VPN server's port range.
        /// </summary>
        public struct PortRange
        {
            /// <summary>
            /// Minimum port.
            /// </summary>
            public int From;

            /// <summary>
            /// Maximum port.
            /// </summary>
            public int To;

            /// <summary>
            /// Total number of ports.
            /// </summary>
            public int Count;

            /// <summary>
            /// Likelihood of the port range being picked during random selection, based on the total number of ports in the port range.
            /// </summary>
            public uint Weight;
        }
    }
}
