// <copyright file="Broker.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Class <c>Broker</c> models the intermediary third-party that manages transactions between the FirefoxPrivateNetwork
    /// VPN client and the WireGuard tunnel.
    /// </summary>
    internal class Broker
    {
        private const int ChildProcessTimeoutSeconds = 5;
        private static Process brokerProcess;

        private IntPtr masterReadPipe = IntPtr.Zero;
        private IntPtr masterWritePipe = IntPtr.Zero;

        private WireGuard.IPC brokerIPC;

        private bool isPipeActive;
        private int remoteBrokerPid;
        private DateTime lastReceivedHeartBeat = DateTime.MinValue;

        /// <summary>
        /// Runs and elevates a process with a UAC popup.
        /// </summary>
        /// <param name="program">Binary to execute.</param>
        /// <param name="arguments">Command line arguments for execution.</param>
        /// <returns>True on success.</returns>
        public static bool UACShellExecute(string program, string arguments)
        {
            brokerProcess = new Process()
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    Verb = "runas",
                    FileName = program,
                    Arguments = arguments,
                },
            };

            return brokerProcess.Start();
        }

        /// <summary>
        /// Child process thread which will be executed upon calling the FirefoxPrivateNetwork.exe with the "broker" switch.
        /// </summary>
        /// <param name="parentPID">Parent process ID.</param>
        /// <param name="readPipeHandle">hWnd of the read pipe.</param>
        /// <param name="writePipeHandle">hWnd of the write pipe.</param>
        /// <returns>Process success result.</returns>
        public static bool ChildProcess(int parentPID, string readPipeHandle, string writePipeHandle)
        {
            // Grab the main app (parent) process based on the provided process ID
            Process parentProcess;
            try
            {
                parentProcess = Process.GetProcessById(parentPID);

                // Make sure we check if the process is active and responding
                if (!parentProcess.Responding)
                {
                    ErrorHandling.ErrorHandler.Handle("Parent process not responding", ErrorHandling.LogLevel.Error);
                    return false;
                }
            }
            catch (ArgumentException e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            // Duplicate the read and write pipe handles from the main app (parent) so that this process (child) owns it
            IntPtr parentProcessHandle = parentProcess.Handle;

            var handleStatusRead = Kernel32.DuplicateHandle(parentProcessHandle, new IntPtr(long.Parse(readPipeHandle)), Process.GetCurrentProcess().Handle, out IntPtr brokerReadPipe, 0, false, (uint)DuplicateOptions.DUPLICATE_SAME_ACCESS | (uint)DuplicateOptions.DUPLICATE_CLOSE_SOURCE);
            var handleStatusWrite = Kernel32.DuplicateHandle(parentProcessHandle, new IntPtr(long.Parse(writePipeHandle)), Process.GetCurrentProcess().Handle, out IntPtr brokerWritePipe, 0, false, (uint)DuplicateOptions.DUPLICATE_SAME_ACCESS | (uint)DuplicateOptions.DUPLICATE_CLOSE_SOURCE);

            // Check if the duplication was successful
            if (!handleStatusRead || !handleStatusWrite)
            {
                ErrorHandling.ErrorHandler.Handle("Failure in creating read/write pipes", ErrorHandling.LogLevel.Error);
                return false;
            }

            // Main child process loop - start the listener thread and listen for commands
            var ipc = new WireGuard.IPC(brokerReadPipe, brokerWritePipe);
            ipc.StartListenerThread();

            // If the listener is not active, sleep for 1 second and wait until it does become active
            while (ipc.IsListenerActive())
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            return true;
        }

        /// <summary>
        /// Launches the broker process in an elevated form (with a UAC popup).
        /// </summary>
        /// <returns>True on successful broker launch, false otherwise.</returns>
        public bool LaunchChildProcess()
        {
            ErrorHandling.DebugLogger.LogDebugMsg("Broker process launch initiated");

            // If the broker child process read/write pipes are already active, don't launch
            if ((masterReadPipe != IntPtr.Zero || masterWritePipe != IntPtr.Zero) && IsActive())
            {
                ErrorHandling.DebugLogger.LogDebugMsg("Broker is already active, not launching a new one");
                return false;
            }

            IntPtr brokerReadPipe, brokerWritePipe;

            // No pipes are currently active, proceed to initiate pipes
            try
            {
                // Destroy any existing pipes
                DestroyPipes();
                Kernel32.CreatePipe(out masterReadPipe, out brokerWritePipe, null, 0);
                Kernel32.CreatePipe(out brokerReadPipe, out masterWritePipe, null, 0);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            try
            {
                ErrorHandling.DebugLogger.LogDebugMsg("Broker process elevating");

                // Run this exe with the "broker" option, with the current process ID and the read/write pipe handles
                // This runs the broker process
                bool runSuccess = false;
                try
                {
                    if (UACShellExecute(Application.ExecutablePath, string.Format("broker {0} {1} {2}", Process.GetCurrentProcess().Id, brokerReadPipe, brokerWritePipe)))
                    {
                        // Successfully launched broker
                        runSuccess = true;
                    }
                    else
                    {
                        ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-vpn-start-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-user-acess-control-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowNotice, ErrorHandling.LogLevel.Info);
                }
                catch (Exception)
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-vpn-start-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                }

                // Failure to run elevated broker process, destroy pipes and exit
                if (!runSuccess)
                {
                    DestroyPipes();
                    return false;
                }

                // Start the pipe listener thread
                brokerIPC = new WireGuard.IPC(masterReadPipe, masterWritePipe);
                brokerIPC.StartListenerThread();

                // Process has been successfully started, but we need to wait until it becomes active
                var waitCounter = 0;

                while (!IsActive(false) && waitCounter++ < ChildProcessTimeoutSeconds * 10)
                {
                    if (waitCounter % 10 == 0)
                    {
                        ErrorHandling.DebugLogger.LogDebugMsg("Waiting on broker to send heartbeat");
                    }

                    Thread.Sleep(100);
                }

                if (!IsActive(false))
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-vpn-launch-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                    Manager.MainWindowViewModel.Status = Models.ConnectionState.Unprotected;
                    return false;
                }
                else
                {
                    // Start the broker <-> main app checker thread
                    var pipeStatusCheckProcess = new Thread(BrokerStatusCheckThread)
                    {
                        IsBackground = true,
                    };

                    ReportHeartbeat();
                    pipeStatusCheckProcess.Start();
                }

                ErrorHandling.DebugLogger.LogDebugMsg("Broker process launched");
            }
            catch (Exception e)
            {
                DestroyPipes();
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the Broker process is running or not.
        /// </summary>
        /// <param name="checkBrokerPipe">Try to detect whether the broker pipe is active before returning.</param>
        /// <returns>True if process is running.</returns>
        public bool IsActive(bool checkBrokerPipe = true)
        {
            try
            {
                if (brokerIPC == null)
                {
                    return false;
                }

                if (checkBrokerPipe && !isPipeActive)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Thread for waiting on the broker to send a PID and then checking whether the the broker is still active.
        /// </summary>
        public void BrokerStatusCheckThread()
        {
            int strikes = 0;
            var pidRequestMessage = new IPCMessage(IPCCommand.IpcRequestPid);

            isPipeActive = true;
            remoteBrokerPid = 0;

            while (true)
            {
                Thread.Sleep(1000);

                // Do we have a broker process ID? If so, break and wait
                if (remoteBrokerPid > 0)
                {
                    isPipeActive = true;
                    break;
                }

                // Did we even attempt to initialize the broker?
                if (brokerIPC == null)
                {
                    isPipeActive = false;
                    continue;
                }

                // Try to send a PID request message
                if (!brokerIPC.WriteToPipe(pidRequestMessage))
                {
                    isPipeActive = false;
                    continue;
                }

                // Check for any previously received PIDs
                if ((DateTime.Now - lastReceivedHeartBeat).TotalSeconds > ProductConstants.BrokerToubleGracePeriod)
                {
                    // No heartbeat received for at least ProductConstants.BrokerToubleGracePeriod seconds, try to recover
                    if (++strikes >= 3)
                    {
                        ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-vpn-start-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.LogLevel.Error);
                        ErrorHandling.ErrorHandler.Handle(string.Format("Broker timed out, no heartbeat received for {0} seconds.", ProductConstants.BrokerToubleGracePeriod), ErrorHandling.LogLevel.Error);
                        break;
                    }

                    continue;
                }
            }

            // If the broker is running, wait on exit
            if (remoteBrokerPid > 0)
            {
                ErrorHandling.ErrorHandler.WriteToLog("Broker is running. Waiting on exit.", ErrorHandling.LogLevel.Info);

                var remoteBrokerProcess = Process.GetProcessById(remoteBrokerPid);
                remoteBrokerProcess.WaitForExit();
                remoteBrokerPid = 0;

                ErrorHandling.ErrorHandler.WriteToLog("Broker has exited.", ErrorHandling.LogLevel.Info);
            }

            // Nothing seems to be running, destroy pipes and move on
            DestroyPipes();
            brokerIPC = null;
        }

        /// <summary>
        /// Fetches the broker IPC listener instance.
        /// </summary>
        /// <returns>IPC instance.</returns>
        public WireGuard.IPC GetBrokerIPC()
        {
            return brokerIPC;
        }

        /// <summary>
        /// Sets the last received heartbeat time to DateTime.Now.
        /// </summary>
        public void ReportHeartbeat()
        {
            lastReceivedHeartBeat = DateTime.Now;
        }

        /// <summary>
        /// Sets the instantiated broker process ID so we can monitor it.
        /// </summary>
        /// <param name="pid">Process ID of the running broker.</param>
        public void SetRemoteBrokerPid(int pid)
        {
            remoteBrokerPid = pid;
        }

        /// <summary>
        /// Resets the last received heartbeat time to a minimum value.
        /// </summary>
        public void ClearHeartbeat()
        {
            lastReceivedHeartBeat = DateTime.MinValue;
        }

        /// <summary>
        /// Shut down the broker.
        /// </summary>
        public void ShutDown()
        {
            if (brokerProcess != null)
            {
                brokerProcess.Kill();
                brokerProcess.WaitForExit();
            }
        }

        /// <summary>
        /// Closes and disposes the read and write pipe.
        /// </summary>
        private void DestroyPipes()
        {
            if (masterReadPipe != IntPtr.Zero)
            {
                Kernel32.CloseHandle(masterReadPipe);
                masterReadPipe = IntPtr.Zero;
            }

            if (masterWritePipe != IntPtr.Zero)
            {
                Kernel32.CloseHandle(masterWritePipe);
                masterWritePipe = IntPtr.Zero;
            }

            isPipeActive = false;
        }
    }
}
