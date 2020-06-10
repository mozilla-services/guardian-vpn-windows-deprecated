// <copyright file="Broker.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Class <c>Broker</c> models the intermediary third-party that manages transactions between the FirefoxPrivateNetwork
    /// VPN client and the WireGuard tunnel.
    /// </summary>
    internal class Broker
    {
        /// <summary>
        /// Unspecified error when requesting the connection status from the tunnel.
        /// </summary>
        public const int IPCConnectionStatusError = -1;

        private static readonly TimeSpan BrokerServiceTimeout = TimeSpan.FromSeconds(30);
        private static readonly List<Task> ChildProcessList = new List<Task>();

        /// <summary>
        /// Starts the child process in a separate long running task.
        /// </summary>
        public static void StartChildProcess()
        {
            var newChildProcess = Task.Factory.StartNew(() => ChildProcess(), BrokerService.BrokerServiceTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            ChildProcessList.Add(newChildProcess);
        }

        /// <summary>
        /// Stops all spawned child processes and exits the broker service.
        /// </summary>
        public static void StopAllChildProcesses()
        {
            BrokerService.BrokerServiceTokenSource.Cancel();
            IPCHandlers.SignalServiceQueue();

            // Wait until all child processes have quit, but honor the broker service timeout
            Task.WaitAll(ChildProcessList.ToArray(), BrokerServiceTimeout);
        }

        /// <summary>
        /// Shows a toast offering the user the chance to start the Broker service.
        /// </summary>
        public static void PromptRestartBrokerService()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new UI.Components.Toast.Toast(UI.Components.Toast.Style.Error, new ErrorHandling.UserFacingMessage("toast-service-communication-error"), priority: UI.Components.Toast.Priority.Important)
                {
                    ClickEventHandler = (sender, e) =>
                    {
                        Task.Run(() =>
                        {
                            // Get the broker service from the service controller
                            var brokerService = ServiceController.GetServices().Where(s => s.ServiceName == ProductConstants.BrokerServiceName).FirstOrDefault();

                            // Start a net process to restart the broker service
                            if (brokerService.Status == ServiceControllerStatus.Stopped)
                            {
                                ProcessStartInfo info = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    Verb = "runas",
                                    FileName = "net",
                                    Arguments = string.Join(" ", new string[] { "start", ProductConstants.BrokerServiceName }),
                                };

                                try
                                {
                                    Process.Start(info);
                                }
                                catch (Exception)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Manager.ToastManager.Show(new UI.Components.Toast.Toast(UI.Components.Toast.Style.Error, new ErrorHandling.UserFacingMessage("toast-service-restart-error"), duration: TimeSpan.FromSeconds(5), priority: UI.Components.Toast.Priority.Important));
                                    });
                                }
                            }

                            // Wait for the broker service to run
                            brokerService.WaitForStatus(ServiceControllerStatus.Running, BrokerServiceTimeout);

                            if (brokerService.Status == ServiceControllerStatus.Running)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Manager.ToastManager.Show(new UI.Components.Toast.Toast(UI.Components.Toast.Style.Success, new ErrorHandling.UserFacingMessage("toast-service-restart-success"), duration: TimeSpan.FromSeconds(5), priority: UI.Components.Toast.Priority.Important));
                                });
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Manager.ToastManager.Show(new UI.Components.Toast.Toast(UI.Components.Toast.Style.Error, new ErrorHandling.UserFacingMessage("toast-service-restart-error"), duration: TimeSpan.FromSeconds(5), priority: UI.Components.Toast.Priority.Important));
                                });
                            }
                        });
                    },
                };

                Manager.ToastManager.Show(toast);
            });
        }

        /// <summary>
        /// Child process thread which will be executed upon calling the FirefoxPrivateNetwork.exe with the "broker" switch.
        /// </summary>
        private static void ChildProcess()
        {
            // Configure pipe security for the broker pipe
            PipeSecurity ps = new PipeSecurity();
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
            ps.AddAccessRule(new PipeAccessRule(account, PipeAccessRights.ReadWrite, AccessControlType.Allow));
            ps.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().Owner, PipeAccessRights.FullControl, AccessControlType.Allow));

            // Main child process loop - start the listener thread and listen for commands
            using (var brokerPipe = new NamedPipeServerStream(ProductConstants.InternalAppName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 512, 512, ps))
            {
                new IPC(brokerPipe).BrokerListenerThread();
            }
        }
    }
}
