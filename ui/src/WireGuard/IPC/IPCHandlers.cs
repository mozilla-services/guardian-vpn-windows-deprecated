// <copyright file="IPCHandlers.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
                while (ServiceQueueEvent.WaitOne())
                {
                    while (ServiceQueue.TryDequeue(out ServiceQueueMessage m))
                    {
                        if (m.Ipc == null)
                        {
                            HandleDisconnect();
                        }
                        else
                        {
                            HandleConnect(m.Message, m.Ipc);
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

                case IPCCommand.IpcDisconnect:
                    if (ServiceQueueRunner.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        ServiceQueueRunner.Start();
                    }

                    ServiceQueue.Enqueue(new ServiceQueueMessage());
                    ServiceQueueEvent.Set();
                    break;

                case IPCCommand.IpcDetectCaptivePortal:
                    HandleIPCDetectCaptivePortal(ipc);
                    break;

                case IPCCommand.IpcRequestPid:
                    HandleIPCPidRequest(ipc);
                    break;

                case IPCCommand.IpcPidReply:
                    HandleIPCPidReply(cmd);
                    break;

                case IPCCommand.IpcConnectReply:
                    HandleIPCConnectReply(cmd);
                    break;

                case IPCCommand.IpcDetectCaptivePortalReply:
                    HandleIPCDetectCaptivePortalReply(cmd);
                    break;
            }
        }

        /// <summary>
        /// Attempt to connect to a tunnel.
        /// </summary>
        /// <param name="cmd">IPCMessage object containing commands and parameters.</param>
        /// <param name="ipc">IPC object containing an instance which lets us reply.</param>
        private static void HandleConnect(IPCMessage cmd, IPC ipc)
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
                errorReply.AddAttribute("error_code", serviceStartResult.ErrorCode.ToString());
                errorReply.AddAttribute("error_description", ((WireGuardTunnelExitCodes)serviceStartResult.ErrorCode).ToString());
                ipc.WriteToPipe(errorReply);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }
        }

        /// <summary>
        /// Stops the tunnel service and deletes it.
        /// </summary>
        private static void HandleDisconnect()
        {
            Service.StopAndDelete();
        }

        private static void HandleIPCPidRequest(IPC ipc)
        {
            var pidReply = new IPCMessage(IPCCommand.IpcPidReply);
            pidReply.AddAttribute("pid", Process.GetCurrentProcess().Id.ToString());
            ipc.WriteToPipe(pidReply);
        }

        private static void HandleIPCPidReply(IPCMessage cmd)
        {
            var pIdString = cmd["pid"].FirstOrDefault();
            if (int.TryParse(pIdString, out int pId))
            {
                Manager.Broker.SetRemoteBrokerPid(pId);
            }
        }

        private static void HandleIPCDetectCaptivePortal(IPC ipc)
        {
            var captivePortalDetectionTask = Network.CaptivePortalDetection.IsCaptivePortalActiveTask();
            captivePortalDetectionTask.ContinueWith(task =>
            {
                var captivePortalDetectionReply = new IPCMessage(IPCCommand.IpcDetectCaptivePortalReply);
                captivePortalDetectionReply.AddAttribute("detected", task.Result == Network.CaptivePortalDetection.ConnectivityStatus.CaptivePortalDetected ? "true" : "false");
                ipc.WriteToPipe(captivePortalDetectionReply);
            });
        }

        private static void HandleIPCDetectCaptivePortalReply(IPCMessage cmd)
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

        private static void HandleIPCConnectReply(IPCMessage cmd)
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
            Manager.Tunnel.Disconnect(false);
        }

        private class ServiceQueueMessage
        {
            public IPCMessage Message { get; set; }

            public IPC Ipc { get; set; }
        }
    }
}
