// <copyright file="Main.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Linq;
using System.Threading;
using FirefoxPrivateNetwork.WCF;
using FirefoxPrivateNetwork.Windows;
using FirefoxPrivateNetwork.WireGuard;

namespace FirefoxPrivateNetwork
{
    /// <summary>
    /// Entry point of the FirefoxPrivateNetwork application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Default C# behavior.")]
    internal class Entry : System.Windows.Application
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

            // Run the broker child process, skip the UI
            if (args.Count() == 4)
            {
                if (args.First().ToLower() == "broker")
                {
                    RunBroker(args);
                    return;
                }
            }

            // Run the tunnel service, skip the UI
            if (args.Count() == 2)
            {
                if (args.First().ToLower() == "tunnel")
                {
                    RunTunnelService(args);
                    return;
                }
            }

            // Run the UI minimized
            if (args.Count() == 1)
            {
                if (args.First().ToLower() == "-s")
                {
                    ranOnStartup = true;
                }
            }

            // Prevent multiple instances of the application
            if (!RunOnceMutex.WaitOne(TimeSpan.Zero, true))
            {
                // Already running, attempt to send a "show" command to the already running process before exiting
                var runningWindow = User32.FindWindow(ProductConstants.TrayWindowClassName, string.Empty);
                if (runningWindow != IntPtr.Zero)
                {
                    User32.SendMessage(runningWindow, User32.SwShow, IntPtr.Zero, string.Empty);
                }

                Environment.Exit(1);
            }

            // We dont need `If Debug_QA` here, because we already had an attribute `[Conditional("DEBUG_QA")]` for this function
            Tester.OpenConnection();

            var staThread = new Thread(() =>
            {
                // Main Application
                var app = new App();
                app.InitializeComponent();

                // Initialize interfaces
                Manager.Initialize();

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
        /// <param name="args">
        /// Argument 0: "broker" keyword
        /// Argument 1: Parent process ID.
        /// Argument 2: Read pipe handle ID.
        /// Argument 3: Write pipe handle ID.
        /// </param>
        private static void RunBroker(string[] args)
        {
            var parentPIDArg = args[1];
            var readPipeHandleArg = args[2];
            var writePipeHandleArg = args[3];

            try
            {
                var parentPID = int.Parse(parentPIDArg);
                var error = FirefoxPrivateNetwork.WireGuard.Broker.ChildProcess(parentPID, readPipeHandleArg, writePipeHandleArg);
                Environment.Exit(error ? 1 : 0);
            }
            catch (FormatException)
            {
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
                // Initialize interfaces
                Manager.InitializeTunnel();

                var error = FirefoxPrivateNetwork.Manager.Tunnel.TunnelService(configFilePath);
                Environment.Exit(0);
            }
            catch (Exception)
            {
                Environment.Exit(1);
            }
        }
    }
}
