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
        /// Global value that is used to indicate if there is already an instance of the application running.
        /// </summary>
        private static readonly Mutex RunOnceMutex = new Mutex(false, string.Concat(@"Local\", ProductConstants.GUID));

        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args">System provided command line parameters.</param>
        public static void Main(string[] args)
        {
            bool ranOnStartup = false;

            List<Process> processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).ToList();

            // Already running, attempt to send a "show" command to the already running process before exiting
            if (processes.Count > 1)
            {
                DateTime placeholderDate = new DateTime(1970, 1, 1);
                processes.Sort((x, y) => DateTime.Compare(x.SessionId == 0 ? placeholderDate : x.StartTime, y.SessionId == 0 ? placeholderDate : y.StartTime));
                for (int i = 0; i < processes.Count - 1; i++)
                {
                    processes[i].CloseMainWindow();
                    processes[i].Close();
                }
            }

            if (args.Count() == 1 && !args[0].Contains("mozilla-vpn:"))
            {
                // Run the broker child process, skip the UI
                if (args.First().ToLower() == "broker")
                {
                    RunBroker();
                    return;
                }

                // Run the UI minimized
                if (args.First().ToLower() == "-s")
                {
                    ranOnStartup = true;
                }
            }

            if (args.Count() == 2)
            {
                // Run the tunnel service, skip the UI
                if (args.First().ToLower() == "tunnel")
                {
                    RunTunnelService(args);
                    return;
                }
            }

            // We dont need `If Debug_QA` here, because we already had an attribute `[Conditional("DEBUG_QA")]` for this function
            Tester.OpenConnection();

            var staThread = new Thread(() =>
            {
                // Main Application
                var app = new App();
                app.InitializeComponent();

                // Initialize interfaces
                // Run this if the app is opened via the custom URL protocol with a code
                if (args.Count() == 1 && args[0].Contains("mozilla-vpn:"))
                {
                    // Get the code string that's returned from the server to be used in the verification request
                    Manager.Initialize(args.First());
                    VerifyUser(args.First());
                }
                else
                {
                    Manager.Initialize();
                }

                // Has the app just been launched at Windows startup?
                Manager.MainWindowViewModel.RanOnStartup = ranOnStartup;

                // Run the application
                app.Run();
            })
            {
                Name = "UI Thread",
            };

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
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
        private static void RunTunnelService(string[] args)
        {
            var configFilePath = args[1];

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

        /// <summary>
        /// Starts the WireGuard tunnel service.
        /// </summary>
        /// <param name="args">
        /// Argument 0: "tunnel" keyword
        /// Argument 1: Path to the configuration file.
        /// </param>
        private static void VerifyUser(string args)
        {
            try
            {
                string code = args.Substring(args.IndexOf("code=") + 5);

                if (code != null && code.Length >= 44)
                {
                    FxA.Login verifyUser = new FxA.Login();

                    verifyUser.VerifyUserLogin(code);
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                Environment.Exit(1);
            }
        }
    }
}
