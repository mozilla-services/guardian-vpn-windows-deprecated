// <copyright file="Main.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using FirefoxPrivateNetwork.WCF;
using FirefoxPrivateNetwork.Windows;
using FirefoxPrivateNetwork.WireGuard;

namespace FirefoxPrivateNetwork
{
    /// <summary>
    /// Entry point of the Mozilla VPN application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Default C# behavior.")]
    internal class Entry : Application
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args">System provided command line parameters.</param>
        public static void Main(string[] args)
        {
            // Run the broker child process, skip the UI
            RunBroker();

            // Run the tunnel service, skip the UI
            RunTunnelService();

            return;
        }

        /// <summary>
        /// Starts the broker child process.
        /// </summary>
        private static void RunBroker()
        {
            try
            {
                ServiceBase.Run(new BrokerService());
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Starts the WireGuard tunnel service.
        /// </summary>
        /// <param name="args">
        /// Argument 0: "tunnel" keyword
        /// Argument 1: Path to the configuration file.
        /// </param>
        private static void RunTunnelService()
        {
            var configFilePath = ProductConstants.FirefoxPrivateNetworkConfFile;

            try
            {
                var error = Tunnel.TunnelService(configFilePath);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                Environment.Exit(1);
            }
        }
    }
}
