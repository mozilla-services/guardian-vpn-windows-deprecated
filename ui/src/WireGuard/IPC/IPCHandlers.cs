// <copyright file="IPCHandlers.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Execute procedures based on the command received within a pipe.
    /// </summary>
    internal class IPCHandlers
    {
        private static readonly ConcurrentQueue<ServiceQueueMessage> ServiceQueue = new ConcurrentQueue<ServiceQueueMessage>();
        private static readonly AutoResetEvent ServiceQueueEvent = new AutoResetEvent(false);
        private static readonly Thread ServiceQueueRunner = new Thread(() =>
        {
            try
            {
                while (!BrokerService.BrokerServiceTokenSource.Token.IsCancellationRequested && ServiceQueueEvent.WaitOne())
                {
                    while (ServiceQueue.TryDequeue(out ServiceQueueMessage m))
                    {
                        if (m.Ipc == null)
                        {
                            BrokerHandleDisconnect();
                        }
                        else
                        {
                            BrokerHandleConnect(m.Message, m.Ipc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.WriteToLog(e.ToString(), ErrorHandling.LogLevel.Error);
            }
        });

        /// <summary>
        /// Read the IPCMessage, determine the command name and execute the command (if supported).
        /// </summary>
        /// <param name="cmd">IPCMessage object, containing the command and the parameters.</param>
        /// <param name="ipc">IPC object, containing the pipes which we can use to reply.</param>
        public static void HandleIncomingMessage(IPCMessage cmd, IPC ipc)
        {
            if (cmd.Count < 1)
            {
                return;
            }

            switch (cmd.First().Item1 + "=" + cmd.First().Item2)
            {
                case IPCCommand.IpcConnect:
                    if (ServiceQueueRunner.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        ServiceQueueRunner.Start();
                    }

                    ServiceQueue.Enqueue(new ServiceQueueMessage { Message = cmd, Ipc = ipc });
                    ServiceQueueEvent.Set();
                    break;

                case IPCCommand.IpcConnectReply:
                    ClientHandleIPCConnectReply(cmd);
                    break;

                case IPCCommand.IpcDisconnect:
                    if (ServiceQueueRunner.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        ServiceQueueRunner.Start();
                    }

                    ServiceQueue.Enqueue(new ServiceQueueMessage());
                    ServiceQueueEvent.Set();
                    break;

                case IPCCommand.IpcConnectionStatus:
                    BrokerHandleConnectionStatus(ipc);
                    break;

                case IPCCommand.IpcConnectionStatusReply:
                    ClientHandleConnectionStatusReply(cmd);
                    break;

                case IPCCommand.IpcDetectCaptivePortal:
                    BrokerHandleIPCDetectCaptivePortal(cmd, ipc);
                    break;

                case IPCCommand.IpcDetectCaptivePortalReply:
                    ClientHandleIPCDetectCaptivePortalReply(cmd);
                    break;
            }
        }

        /// <summary>
        /// Set the service queue event so that the WaitOne() is unblocked within the service queue runner.
        /// </summary>
        public static void SignalServiceQueue()
        {
            ServiceQueueEvent.Set();
        }

        /// <summary>
        /// Attempt to connect to a tunnel.
        /// </summary>
        /// <param name="cmd">IPCMessage object containing commands and parameters.</param>
        /// <param name="ipc">IPC object containing an instance which lets us reply.</param>
        private static void BrokerHandleConnect(IPCMessage cmd, IPC ipc)
        {
            // Install service
            try
            {
                string configFilePath = cmd["config"].FirstOrDefault();
                if (configFilePath == null)
                {
                    return;
                }

                var serviceStartResult = Service.InstallAndRun("\"" + System.AppDomain.CurrentDomain.BaseDirectory + System.AppDomain.CurrentDomain.FriendlyName + "\"" + " tunnel " + "\"" + configFilePath + "\"");
                if (serviceStartResult.Success)
                {
                    return;
                }

                var errorReply = new IPCMessage(IPCCommand.IpcConnectReply);
                errorReply.AddAttribute("error_code", "-1");
                errorReply.AddAttribute("error_description", ((WireGuardTunnelExitCodes)serviceStartResult.ErrorCode).ToString());
                ipc.WriteToPipe(errorReply);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }
        }

        private static void ClientHandleIPCConnectReply(IPCMessage cmd)
        {
            var errorCodeString = cmd["error_code"].FirstOrDefault();
            if (errorCodeString == null)
            {
                return;
            }

            if (!int.TryParse(errorCodeString, out int errorCode))
            {
                return;
            }

            Connector.HandleTunnelFailure((WireGuardTunnelExitCodes)errorCode);
            Manager.Tunnel.Disconnect();
        }

        /// <summary>
        /// Stops the tunnel service and deletes it.
        /// </summary>
        private static void BrokerHandleDisconnect()
        {
            Service.StopAndDelete();
        }

        private static void BrokerHandleConnectionStatus(IPC ipc)
        {
            var connectionStatus = BrokerQueryConnectionStatisticsFromTunnel();
            var connectionStatusReply = new IPCMessage(IPCCommand.IpcConnectionStatusReply);

            if (connectionStatus == null)
            {
                connectionStatusReply.AddAttribute("broker_error_code", Broker.IPCConnectionStatusError.ToString());
            }
            else
            {
                connectionStatusReply.AddRange(connectionStatus);
            }

            ipc.WriteToPipe(connectionStatusReply);
        }

        private static void ClientHandleConnectionStatusReply(IPCMessage cmd)
        {
            var connectionStatus = Manager.Tunnel.ParseStatusResponse(cmd);
            Manager.ConnectionStatusUpdater.UpdateConnectionStatus(connectionStatus);
            Manager.ConnectionStatusUpdater.RequestConnectionStatusTcs.TrySetResult(true);
        }

        private static void BrokerHandleIPCDetectCaptivePortal(IPCMessage cmd, IPC ipc)
        {
            var captivePortalDetectionTask = Network.CaptivePortalDetection.IsCaptivePortalActiveTask(cmd["ip"].FirstOrDefault());

            captivePortalDetectionTask.ContinueWith(task =>
            {
                var captivePortalDetectionReply = new IPCMessage(IPCCommand.IpcDetectCaptivePortalReply);
                captivePortalDetectionReply.AddAttribute("detected", task.Result == Network.CaptivePortalDetection.ConnectivityStatus.CaptivePortalDetected ? "true" : "false");
                ipc.WriteToPipe(captivePortalDetectionReply);
            });
        }

        private static void ClientHandleIPCDetectCaptivePortalReply(IPCMessage cmd)
        {
            var captivePortalDetected = cmd["detected"].FirstOrDefault();
            if (captivePortalDetected == "true")
            {
                Manager.CaptivePortalDetector.CaptivePortalDetected = true;
            }
            else
            {
                Manager.CaptivePortalDetector.CaptivePortalDetected = false;
            }
        }

        /// <summary>
        /// Send a request for connection statistics from the tunnel service via named pipe.
        /// </summary>
        /// <returns>Connection status received from the tunnel named pipe.</returns>
        private static IPCMessage BrokerQueryConnectionStatisticsFromTunnel()
        {
            NamedPipeClientStream tunnelPipe = null;
            try
            {
                tunnelPipe = BrokerConnectWGTunnelNamedPipe();
                if (tunnelPipe != null)
                {
                    IPC.WriteToPipe(tunnelPipe, new IPCMessage(IPCCommand.WgGet));
                    var ret = IPC.ReadFromPipe(tunnelPipe);
                    tunnelPipe.Close();
                    return ret;
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
            }
            finally
            {
                if (tunnelPipe != null && tunnelPipe.IsConnected)
                {
                    tunnelPipe.Close();
                }
            }

            return null;
        }

        private static NamedPipeClientStream BrokerConnectWGTunnelNamedPipe()
        {
            try
            {
                var tunnelPipe = new NamedPipeClientStream(ProductConstants.TunnelPipeName);
                tunnelPipe.Connect(1000);
                return tunnelPipe;
            }
            catch (System.TimeoutException)
            {
                if (Manager.MainWindowViewModel.Status == Models.ConnectionState.Protected)
                {
                    ErrorHandling.ErrorHandler.Handle("Named pipe not available", ErrorHandling.LogLevel.Debug);
                }

                return null;
            }
        }

        private class ServiceQueueMessage
        {
            public IPCMessage Message { get; set; }

            public IPC Ipc { get; set; }
        }
    }
}
