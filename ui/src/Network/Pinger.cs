// <copyright file="Pinger.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace FirefoxPrivateNetwork.Network
{
    /// <summary>
    /// ICMP packet sender. Used for sending ICMP requests to the first hop during the VPN connection in order to measure connection stability.
    /// </summary>
    internal class Pinger
    {
        /// <summary>
        /// Pings an IP address asynchronously, doesn't wait for a reply.
        /// </summary>
        /// <param name="ip">IP address to ping.</param>
        public static void Ping(string ip)
        {
            using (var pinger = new Ping())
            {
                var pingOptions = new PingOptions(64, false);

                int timeout = 5000;
                string pingData = ProductConstants.ProductName.PadRight(32, '.').Substring(0, 32);
                byte[] pingBuffer = Encoding.ASCII.GetBytes(pingData);

                try
                {
                    pinger.SendAsync(ip, timeout, pingBuffer, pingOptions);
                }
                catch (Exception)
                {
                    // Failure to ping is not an issue in this case
                }
            }
        }

        /// <summary>
        /// Starts the pinger thread.
        /// </summary>
        public void StartThread()
        {
            var pingUpdater = new Thread(() => PingCurrentGateway())
            {
                IsBackground = true,
            };
            pingUpdater.Start();
        }

        /// <summary>
        /// If connection is active, pings the currently used gateway/dns server from the server list.
        /// </summary>
        private void PingCurrentGateway()
        {
            try
            {
                while (true)
                {
                    if (Manager.MainWindowViewModel.Status == Models.ConnectionState.Protected)
                    {
                        var currentServer = FxA.Cache.FxAServerList.GetServerByIP(Manager.MainWindowViewModel.ServerSelected.Endpoint);
                        Ping(currentServer.DNSServerAddress);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
            }
        }
    }
}
