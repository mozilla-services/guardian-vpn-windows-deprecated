// <copyright file="IPCHandlers.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

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

                case IPCCommand.IpcApplyNetworkFilters:
                    BrokerHandleApplyNetworkFilters(ipc);
                    break;

                case IPCCommand.IpcApplyNetworkFiltersReply:
                    ClientHandleApplyNetworkFiltersReply(cmd);
                    break;

                case IPCCommand.IpcRemoveNetworkFilters:
                    BrokerHandleRemoveNetworkFilters(ipc, cmd);
                    break;

                case IPCCommand.IpcRemoveNetworkFiltersReply:
                    ClientHandleRemoveNetworkFiltersReply();
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

        private static void BrokerHandleApplyNetworkFilters(IPC ipc)
        {
            // Open a session to the filter engine
            IntPtr engineHandle = IntPtr.Zero;
            Windows.Fwpuclnt.FWPM_SESSION0_ session = default;
            IntPtr sessionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Windows.Fwpuclnt.FWPM_SESSION0_)));
            Marshal.StructureToPtr(session, sessionPtr, false);

            var applyNetworkFiltersReply = new IPCMessage(IPCCommand.IpcApplyNetworkFiltersReply);

            try
            {
                var error = Windows.Fwpuclnt.NativeMethods.FwpmEngineOpen0(null, 10, IntPtr.Zero, sessionPtr, ref engineHandle);
                Marshal.ThrowExceptionForHR(error);

                Guid sublayerKey = Guid.NewGuid();

                Windows.Fwpuclnt.FWPM_SUBLAYER0_ sublayer = new Windows.Fwpuclnt.FWPM_SUBLAYER0_
                {
                    subLayerKey = sublayerKey,
                    displayData = new Windows.Fwpuclnt.FWPM_DISPLAY_DATA0_
                    {
                        name = "Firefox Private Network Sublayer",
                        description = "Sublayer for IPv4 and IPv6 filters",
                    },
                    weight = 0,
                };

                error = Windows.Fwpuclnt.NativeMethods.FwpmSubLayerAdd0(engineHandle, ref sublayer, IntPtr.Zero);
                Marshal.ThrowExceptionForHR(error);

                applyNetworkFiltersReply.AddAttribute("sublayerKey", sublayerKey.ToString());

                Guid ipv4FilterKey = Guid.NewGuid();
                var ipv4Filter = GetBlockingFilter(ipv4FilterKey, sublayerKey, Windows.Fwpuclnt.FWPM_LAYER_ALE_AUTH_CONNECT_V4);
                error = Windows.Fwpuclnt.NativeMethods.FwpmFilterAdd0(engineHandle, ref ipv4Filter, IntPtr.Zero, ref ipv4Filter.filterId);
                Marshal.ThrowExceptionForHR(error);

                applyNetworkFiltersReply.AddAttribute("ipv4FilterKey", ipv4FilterKey.ToString());

                Guid ipv6FilterKey = Guid.NewGuid();
                var ipv6Filter = GetBlockingFilter(ipv6FilterKey, sublayerKey, Windows.Fwpuclnt.FWPM_LAYER_ALE_AUTH_CONNECT_V6);
                error = Windows.Fwpuclnt.NativeMethods.FwpmFilterAdd0(engineHandle, ref ipv6Filter, IntPtr.Zero, ref ipv6Filter.filterId);
                Marshal.ThrowExceptionForHR(error);

                applyNetworkFiltersReply.AddAttribute("ipv6FilterKey", ipv6FilterKey.ToString());

                error = Windows.Fwpuclnt.NativeMethods.FwpmEngineClose0(engineHandle);
                Marshal.ThrowExceptionForHR(error);
            }
            catch
            {
            }
            finally
            {
                ipc.WriteToPipe(applyNetworkFiltersReply);
            }
        }

        private static Windows.Fwpuclnt.FWPM_FILTER0_ GetBlockingFilter(Guid filterKey, Guid sublayerKey, Guid layerKey)
        {
            string displayDataName = string.Empty;
            string displayDataDescription = string.Empty;

            if (layerKey == Windows.Fwpuclnt.FWPM_LAYER_ALE_AUTH_CONNECT_V4)
            {
                displayDataName = "IPv4 filter";
                displayDataDescription = "Blocks all network traffic using IPv4";
            }
            else if (layerKey == Windows.Fwpuclnt.FWPM_LAYER_ALE_AUTH_CONNECT_V6)
            {
                displayDataName = "IPv6 filter";
                displayDataDescription = "Blocks all network traffic using IPv6";
            }

            var filter = new Windows.Fwpuclnt.FWPM_FILTER0_
            {
                filterKey = filterKey,
                layerKey = layerKey,
                subLayerKey = sublayerKey,
                displayData = new Windows.Fwpuclnt.FWPM_DISPLAY_DATA0_
                {
                    name = displayDataName,
                    description = displayDataDescription,
                },
                action = new Windows.Fwpuclnt.FWPM_ACTION0_
                {
                    type = Windows.Fwpuclnt.FWP_ACTION_BLOCK,
                },
                weight = new Windows.Fwpuclnt.FWP_VALUE0_
                {
                    type = Windows.Fwpuclnt.FWP_DATA_TYPE_.FWP_EMPTY,
                },
            };
            return filter;
        }

        private static void ClientHandleApplyNetworkFiltersReply(IPCMessage cmd)
        {
            List<string> key = cmd["sublayerKey"];
            if (key.Count > 0)
            {
                Guid.TryParse(key.FirstOrDefault(), out Guid sublayerKeyGuid);
                Manager.SublayerKey = sublayerKeyGuid;
            }

            key = cmd["ipv4FilterKey"];
            if (key.Count > 0)
            {
                Guid.TryParse(key.FirstOrDefault(), out Guid ipv4FilterKeyGuid);
                Manager.Ipv4FilterKey = ipv4FilterKeyGuid;
            }

            key = cmd["ipv6FilterKey"];
            if (key.Count > 0)
            {
                Guid.TryParse(key.FirstOrDefault(), out Guid ipv6FilterKeyGuid);
                Manager.Ipv6FilterKey = ipv6FilterKeyGuid;
            }
        }

        private static void BrokerHandleRemoveNetworkFilters(IPC ipc, IPCMessage cmd)
        {
            bool removeSublayer = Guid.TryParse(cmd["sublayerKey"].FirstOrDefault(), out var sublayerKey);
            bool removeIpv4Filter = Guid.TryParse(cmd["ipv4FilterKey"].FirstOrDefault(), out var ipv4FilterKey);
            bool removeIpv6Filter = Guid.TryParse(cmd["ipv6FilterKey"].FirstOrDefault(), out var ipv6FilterKey);

            IntPtr engineHandle = IntPtr.Zero;
            Windows.Fwpuclnt.FWPM_SESSION0_ session = default;
            IntPtr sessionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Windows.Fwpuclnt.FWPM_SESSION0_)));
            Marshal.StructureToPtr(session, sessionPtr, false);

            try
            {
                var error = Windows.Fwpuclnt.NativeMethods.FwpmEngineOpen0(null, 10, IntPtr.Zero, sessionPtr, ref engineHandle);
                Marshal.ThrowExceptionForHR(error);

                if (removeIpv6Filter)
                {
                    error = Windows.Fwpuclnt.NativeMethods.FwpmFilterDeleteByKey0(engineHandle, ref ipv6FilterKey);
                    Marshal.ThrowExceptionForHR(error);
                }

                if (removeIpv4Filter)
                {
                    error = Windows.Fwpuclnt.NativeMethods.FwpmFilterDeleteByKey0(engineHandle, ref ipv4FilterKey);
                    Marshal.ThrowExceptionForHR(error);
                }

                if (removeSublayer)
                {
                    error = Windows.Fwpuclnt.NativeMethods.FwpmSubLayerDeleteByKey0(engineHandle, ref sublayerKey);
                    Marshal.ThrowExceptionForHR(error);
                }

                error = Windows.Fwpuclnt.NativeMethods.FwpmEngineClose0(engineHandle);
                Marshal.ThrowExceptionForHR(error);
            }
            catch
            {
            }

            var removeNetworkFiltersReply = new IPCMessage(IPCCommand.IpcRemoveNetworkFiltersReply);
            ipc.WriteToPipe(removeNetworkFiltersReply);
        }

        private static void ClientHandleRemoveNetworkFiltersReply()
        {
            Manager.SublayerKey = Guid.Empty;
            Manager.Ipv4FilterKey = Guid.Empty;
            Manager.Ipv6FilterKey = Guid.Empty;
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
