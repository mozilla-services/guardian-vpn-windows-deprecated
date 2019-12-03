// <copyright file="Service.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Windows Service functionality methods.
    /// </summary>
    internal class Service
    {
        private static bool serviceStopping = false;

        /// <summary>
        /// Installs the tunnel service and starts it up.
        /// </summary>
        /// <param name="servicePath">Path to the service EXE to run, along with optional command line parameters.</param>
        /// <returns>ServiceStartResult with startup status and error code.</returns>
        public static ServiceStartResult InstallAndRun(string servicePath)
        {
            if (serviceStopping)
            {
                return new ServiceStartResult { Success = false, ErrorCode = 0 };
            }

            IntPtr serviceManager = Windows.AdvApi32.OpenSCManager(null, null, Windows.ScmAccessRights.AllAccess);

            try
            {
                // Try and open the service with all access rights
                IntPtr service = Windows.AdvApi32.OpenService(serviceManager, ProductConstants.TunnelServiceInternalName, Windows.ServiceAccessRights.AllAccess);

                // Do we have a service at this point?
                if (service != IntPtr.Zero)
                {
                    Windows.AdvApi32.CloseServiceHandle(service);
                    StopAndDelete();
                }

                // Create a new service and start it
                service = Windows.AdvApi32.CreateService(serviceManager, ProductConstants.TunnelServiceInternalName, ProductConstants.TunnelServiceName, Windows.ServiceAccessRights.AllAccess, (int)ServiceType.Win32OwnProcess, Windows.ServiceStartType.DemandStart, Windows.ServiceError.Normal, servicePath, null, IntPtr.Zero, "Nsi", null, null);

                if (service == IntPtr.Zero)
                {
                    return new ServiceStartResult { Success = false, ErrorCode = 0 };
                }

                var serviceDescription = new Windows.ServiceDescription
                {
                    Description = ProductConstants.TunnelServiceDescription,
                };

                // Set the SID type and the service description
                var serviceSidType = Windows.ServiceSidType.Unrestricted;
                Windows.AdvApi32.ChangeServiceConfig2(service, Windows.AdvApi32.ServiceConfigDescriptionFlag, ref serviceDescription);
                Windows.AdvApi32.ChangeServiceConfig2(service, Windows.AdvApi32.ServiceConfigSidInfoFlag, ref serviceSidType);

                try
                {
                    // Attempt to start the service
                    Windows.AdvApi32.StartService(service, 0, new string[] { });

                    if (!WaitForServiceStatus(service, Windows.ServiceState.Running))
                    {
                        // Fetch the service status
                        var serviceStatus = default(Windows.ServiceStatus);
                        Windows.AdvApi32.QueryServiceStatus(service, ref serviceStatus);
                        ErrorHandling.ErrorHandler.Handle(string.Concat("Could not start tunnel service, exited with error code: ", serviceStatus.DwWin32ExitCode), ErrorHandling.LogLevel.Error);

                        StopAndDelete();
                        return new ServiceStartResult { Success = false, ErrorCode = serviceStatus.DwServiceSpecificExitCode };
                    }
                }
                finally
                {
                    Windows.AdvApi32.CloseServiceHandle(service);
                }
            }
            finally
            {
                Windows.AdvApi32.CloseServiceHandle(serviceManager);
            }

            return new ServiceStartResult { Success = true, ErrorCode = 0 };
        }

        /// <summary>
        /// Checks whether the service is running or not.
        /// </summary>
        /// <returns>Returns true if the service is running.</returns>
        public static bool IsTunnelServiceRunning()
        {
            if (serviceStopping)
            {
                return true;
            }

            bool status = false;

            IntPtr serviceManager = IntPtr.Zero;
            IntPtr service = IntPtr.Zero;

            try
            {
                serviceManager = Windows.AdvApi32.OpenSCManager(null, null, Windows.ScmAccessRights.EnumerateService);

                // Try and open the service with all access rights
                service = Windows.AdvApi32.OpenService(serviceManager, ProductConstants.TunnelServiceInternalName, Windows.ServiceAccessRights.QueryStatus);

                // Have we managed to open a service?
                if (service != IntPtr.Zero)
                {
                    var serviceStatus = default(Windows.ServiceStatus);

                    // Fetch the service status
                    Windows.AdvApi32.QueryServiceStatus(service, ref serviceStatus);
                    if (serviceStatus.DwCurrentState == Windows.ServiceState.Running)
                    {
                        status = true;
                    }
                }
            }
            finally
            {
                if (service != IntPtr.Zero)
                {
                    Windows.AdvApi32.CloseServiceHandle(service);
                }

                if (serviceManager != IntPtr.Zero)
                {
                    Windows.AdvApi32.CloseServiceHandle(serviceManager);
                }
            }

            return status;
        }

        /// <summary>
        /// Stop the tunnel service and delete it.
        /// </summary>
        public static void StopAndDelete()
        {
            serviceStopping = true;

            IntPtr serviceManager = Windows.AdvApi32.OpenSCManager(null, null, Windows.ScmAccessRights.AllAccess);
            try
            {
                IntPtr service = Windows.AdvApi32.OpenService(serviceManager, ProductConstants.TunnelServiceInternalName, Windows.ServiceAccessRights.AllAccess);

                // Have we managed to open the service?
                if (service != IntPtr.Zero)
                {
                    try
                    {
                        var serviceStatus = default(Windows.ServiceStatus);

                        Windows.AdvApi32.QueryServiceStatus(service, ref serviceStatus);
                        if (serviceStatus.DwCurrentState != Windows.ServiceState.Stopped)
                        {
                            // Wait for the service to stop
                            var controlServiceResult = Windows.AdvApi32.ControlService(service, Windows.ServiceControl.Stop, serviceStatus);
                            var hasStatusChanged = WaitForServiceStatus(service, Windows.ServiceState.Stopped);

                            if (!hasStatusChanged)
                            {
                                ErrorHandling.ErrorHandler.Handle("Could not stop the service", ErrorHandling.LogLevel.Error);
                            }
                        }

                        if (!Windows.AdvApi32.DeleteService(service))
                        {
                            ErrorHandling.ErrorHandler.Handle("Could not delete service", ErrorHandling.LogLevel.Error);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                    }
                    finally
                    {
                        Windows.AdvApi32.CloseServiceHandle(service);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }
            finally
            {
                serviceStopping = false;
                Windows.AdvApi32.CloseServiceHandle(serviceManager);
            }
        }

        /// <summary>
        /// Waits for the service to change its status.
        /// </summary>
        /// <param name="service">Pointer to a Windows Service handle.</param>
        /// <param name="desiredStatus">Which status are we expecting the service to have.</param>
        /// <returns>Returns true if the current service status corresponds to the desired status. This is a blocking call.</returns>
        private static bool WaitForServiceStatus(IntPtr service, Windows.ServiceState desiredStatus)
        {
            var status = default(Windows.ServiceStatus);

            int tries = 0;
            int maxWaitTries = 30;

            // Check the status for the first time
            Windows.AdvApi32.QueryServiceStatus(service, ref status);

            while (status.DwCurrentState != desiredStatus)
            {
                // Check if the service has failed to start
                if (status.DwWin32ExitCode != 0)
                {
                    break;
                }

                // Check the status
                if (Windows.AdvApi32.QueryServiceStatus(service, ref status) == 0 || tries > maxWaitTries)
                {
                    break;
                }

                tries++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            return status.DwCurrentState == desiredStatus;
        }

        /// <summary>
        /// Struct depicting the result of a service start attempt.
        /// </summary>
        public struct ServiceStartResult
        {
            /// <summary>
            /// Whether the service has successfully started or not.
            /// </summary>
            public bool Success;

            /// <summary>
            /// Error code associated with the service, should it fail to start.
            /// </summary>
            public int ErrorCode;
        }
    }
}
