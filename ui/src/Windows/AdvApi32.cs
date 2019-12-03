// <copyright file="AdvApi32.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// The Windows security model enables access control to the service control manager (SCM) and service objects. This object enumerates the available SCM access rights.
    /// </summary>
    [Flags]
    public enum ScmAccessRights
    {
        /// <summary>
        /// Required to connect to the service control manager.
        /// </summary>
        Connect = 0x0001,

        /// <summary>
        /// Required to call the CreateService function to create a service object and add it to the database.
        /// </summary>
        CreateService = 0x0002,

        /// <summary>
        /// Required to call the EnumServicesStatus or EnumServicesStatusEx function to list the services that are in the database. Required to call the NotifyServiceStatusChange function to receive notification when any service is created or deleted.
        /// </summary>
        EnumerateService = 0x0004,

        /// <summary>
        /// Required to call the LockServiceDatabase function to acquire a lock on the database.
        /// </summary>
        Lock = 0x0008,

        /// <summary>
        /// Required to call the QueryServiceLockStatus function to retrieve the lock status information for the database.
        /// </summary>
        QueryLockStatus = 0x0010,

        /// <summary>
        /// Required to call the NotifyBootConfigStatus function.
        /// </summary>
        ModifyBootConfig = 0x0020,

        /// <summary>
        /// Combines DELETE, READ_CONTROL, WRITE_DAC, and WRITE_OWNER access.
        /// </summary>
        StandardRightsRequired = 0xF0000,

        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights.
        /// </summary>
        AllAccess = StandardRightsRequired | Connect | CreateService | EnumerateService | Lock | QueryLockStatus | ModifyBootConfig,
    }

    /// <summary>
    /// This object enumerates the available service access rights.
    /// </summary>
    [Flags]
    public enum ServiceAccessRights
    {
        /// <summary>
        /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
        /// </summary>
        QueryConfig = 0x1,

        /// <summary>
        /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration.
        /// </summary>
        ChangeConfig = 0x2,

        /// <summary>
        /// Required to call the QueryServiceStatus or QueryServiceStatusEx function to ask the service control manager about the status of the service.
        /// </summary>
        QueryStatus = 0x4,

        /// <summary>
        /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
        /// </summary>
        EnumerateDependants = 0x8,

        /// <summary>
        /// Required to call the StartService function to start the service.
        /// </summary>
        Start = 0x10,

        /// <summary>
        /// Required to call the ControlService function to stop the service.
        /// </summary>
        Stop = 0x20,

        /// <summary>
        /// Required to call the ControlService function to pause or continue the service.
        /// </summary>
        PauseContinue = 0x40,

        /// <summary>
        /// Required to call the ControlService function to ask the service to report its status immediately.
        /// </summary>
        Interrogate = 0x80,

        /// <summary>
        /// Required to call the ControlService function to specify a user-defined control code.
        /// </summary>
        UserDefinedControl = 0x100,

        /// <summary>
        /// Required to call the DeleteService function to delete the service.
        /// </summary>
        Delete = 0x00010000,

        /// <summary>
        /// Combines DELETE, READ_CONTROL, WRITE_DAC, and WRITE_OWNER access.
        /// </summary>
        StandardRightsRequired = 0xF0000,

        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights.
        /// </summary>
        AllAccess = StandardRightsRequired | QueryConfig | ChangeConfig | QueryStatus | EnumerateDependants | Start | Stop | PauseContinue | Interrogate | UserDefinedControl,
    }

    /// <summary>
    /// The service start options.
    /// </summary>
    [Flags]
    public enum ServiceStartType
    {
        /// <summary>
        /// A device driver started by the system loader. This value is valid only for driver services.
        /// </summary>
        BootStart = 0x00000000,

        /// <summary>
        /// A device driver started by the IoInitSystem function. This value is valid only for driver services.
        /// </summary>
        SystemStart = 0x00000001,

        /// <summary>
        /// A service started by the service control manager when a process calls the StartService function.
        /// </summary>
        AutoStart = 0x00000002,

        /// <summary>
        /// A service started by the service control manager when a process calls the StartService function.
        /// </summary>
        DemandStart = 0x00000003,

        /// <summary>
        /// A service that cannot be started.
        /// </summary>
        Disabled = 0x00000004,
    }

    /// <summary>
    /// Service control codes.
    /// </summary>
    [Flags]
    public enum ServiceControl
    {
        /// <summary>
        /// Notifies a service that it should stop.
        /// </summary>
        Stop = 0x00000001,

        /// <summary>
        /// Notifies a service that it should pause
        /// </summary>
        Pause = 0x00000002,

        /// <summary>
        /// Notifies a paused service that it should resume.
        /// </summary>
        Continue = 0x00000003,

        /// <summary>
        /// Notifies a service that it should report its current status information to the service control manager.
        /// </summary>
        Interrogate = 0x00000004,

        /// <summary>
        /// Notifies a service that its startup parameters have changed.
        /// </summary>
        ParamChange = 0x00000006,

        /// <summary>
        /// Notifies a network service that there is a new component for binding.
        /// </summary>
        NetBindAdd = 0x00000007,

        /// <summary>
        /// Notifies a network service that a component for binding has been removed.
        /// </summary>
        NetBindRemove = 0x00000008,

        /// <summary>
        /// Notifies a network service that a disabled binding has been enabled.
        /// </summary>
        NetBindEnable = 0x00000009,

        /// <summary>
        /// Notifies a network service that one of its bindings has been disabled.
        /// </summary>
        NetBindDisable = 0x0000000A,
    }

    /// <summary>
    /// The severity of the error, and action taken, if this service fails to start.
    /// </summary>
    [Flags]
    public enum ServiceError
    {
        /// <summary>
        /// The startup program ignores the error and continues the startup operation.
        /// </summary>
        Ignore = 0x00000000,

        /// <summary>
        /// The startup program logs the error in the event log but continues the startup operation.
        /// </summary>
        Normal = 0x00000001,

        /// <summary>
        /// The startup program logs the error in the event log. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.
        /// </summary>
        Severe = 0x00000002,

        /// <summary>
        /// The startup program logs the error in the event log, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.
        /// </summary>
        Critical = 0x00000003,
    }

    /// <summary>
    /// Security identifier type.
    /// </summary>
    [Flags]
    public enum ServiceSidType
    {
        /// <summary>
        /// None. Use this type to reduce application compatibility issues.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// When the service process is created, the service SID is added to the service process token with the following attributes: SE_GROUP_ENABLED_BY_DEFAULT | SE_GROUP_OWNER.
        /// </summary>
        Unrestricted = 0x00000001,

        /// <summary>
        /// This type includes SERVICE_SID_TYPE_UNRESTRICTED. The service SID is also added to the restricted SID list of the process token.
        /// </summary>
        Restricted = 0x00000003,
    }

    /// <summary>
    /// Service state types for Windows services.
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Service not found.
        /// </summary>
        NotFound = 0,

        /// <summary>
        /// Service is stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// Service start is pending.
        /// </summary>
        StartPending = 2,

        /// <summary>
        /// Service stop is pending.
        /// </summary>
        StopPending = 3,

        /// <summary>
        /// Service is running.
        /// </summary>
        Running = 4,

        /// <summary>
        /// Service is pending to continue.
        /// </summary>
        ContinuePending = 5,

        /// <summary>
        /// Service pause is pending.
        /// </summary>
        PausePending = 6,

        /// <summary>
        /// Service is paused.
        /// </summary>
        Paused = 7,
    }

    /// <summary>
    /// AdvApi32.dll pinvoke library.
    /// </summary>
    public class AdvApi32
    {
        /// <summary>
        /// Option flag indicating that the service description should be changed.
        /// </summary>
        public const int ServiceConfigDescriptionFlag = 1;

        /// <summary>
        /// Option flag indicating that the service security identifier should be changed.
        /// </summary>
        public const int ServiceConfigSidInfoFlag = 5;

        /// <summary>
        /// Establishes a connection to the service control manager on the specified computer and opens the specified service control manager database.
        /// </summary>
        /// <param name="machineName">The name of the target computer. If the pointer is NULL or points to an empty string, the function connects to the service control manager on the local computer.</param>
        /// <param name="databaseName">The name of the service control manager database. This parameter should be set to SERVICES_ACTIVE_DATABASE. If it is NULL, the SERVICES_ACTIVE_DATABASE database is opened by default.</param>
        /// <param name="dwDesiredAccess">The access to the service control manager.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified service control manager database. If the function fails, the return value is NULL.</returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);

        /// <summary>
        /// Opens an existing service.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database.</param>
        /// <param name="lpServiceName">The name of the service to be opened.</param>
        /// <param name="dwDesiredAccess">The access to the service.</param>
        /// <returns>If the function succeeds, the return value is a handle to the service. If the function fails, the return value is NULL.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);

        /// <summary>
        /// Closes a handle to a service control manager or service object.
        /// </summary>
        /// <param name="hSCObject">A handle to the service control manager object or the service object to close.</param>
        /// <returns>If the function succeeds, return value is true.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        /// <summary>
        /// Starts a service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="dwNumServiceArgs">The number of strings in the lpServiceArgVectors array. If lpServiceArgVectors is NULL, this parameter can be zero.</param>
        /// <param name="lpServiceArgVectors">The null-terminated strings to be passed to the ServiceMain function for the service as arguments.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

        /// <summary>
        /// Creates a service object and adds it to the specified service control manager database.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database.</param>
        /// <param name="lpServiceName">The name of the service to install.</param>
        /// <param name="lpDisplayName">The display name to be used by user interface programs to identify the service.</param>
        /// <param name="dwDesiredAccess">The access to the service.</param>
        /// <param name="dwServiceType">The service type.</param>
        /// <param name="dwStartType">The service start options.</param>
        /// <param name="dwErrorControl">The severity of the error, and action taken, if this service fails to start.</param>
        /// <param name="lpBinaryPathName">The fully qualified path to the service binary file.</param>
        /// <param name="lpLoadOrderGroup">The names of the load ordering group of which this service is a member.</param>
        /// <param name="lpdwTagId">A pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter.</param>
        /// <param name="lpDependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service.</param>
        /// <param name="lpServiceStartName">The name of the account under which the service should run.</param>
        /// <param name="lpPassword">The password to the account name specified by the lpServiceStartName parameter.</param>
        /// <returns>If the function succeeds, the return value is a handle to the service. If the function fails, the return value is NULL.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceStartType dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        /// <summary>
        /// Changes the optional configuration parameters of a service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="dwInfoLevel">The configuration information to be changed.</param>
        /// <param name="lpInfo">A pointer to the new value to be set for the configuration information.</param>
        /// <returns>Returns true on success.</returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, ref ServiceSidType lpInfo);

        /// <summary>
        /// Changes the optional configuration parameters of a service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="dwInfoLevel">The configuration information to be changed.</param>
        /// <param name="lpInfo">A pointer to the new value to be set for the configuration information.</param>
        /// <returns>Returns true on success.</returns>
        [DllImport("Advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, ref ServiceDescription lpInfo);

        /// <summary>
        /// Marks the specified service for deletion from the service control manager database.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <returns>Returns true on successful service deletion.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(IntPtr hService);

        /// <summary>
        /// Sends a control code to a service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="dwControl">Control code parameters.</param>
        /// <param name="lpServiceStatus">The ServiceStatus structure that receives the latest service status information.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. The full list of
        /// potential error messages can be found <see href="https://docs.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-controlservice#return-value">here</see>.</returns>
        [DllImport("advapi32.dll")]
        public static extern int ControlService(IntPtr hService, ServiceControl dwControl, ServiceStatus lpServiceStatus);

        /// <summary>
        /// Retrieves the current status of the specified service.
        /// </summary>
        /// <param name="hService">A handle to the service.</param>
        /// <param name="lpServiceStatus">A pointer to the ServiceStatus structure that receives the status information.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. The full list of
        /// potential error messages can be found <see href="https://docs.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-queryservicestatus#return-value">here</see>.</returns>
        [DllImport("advapi32.dll")]
        public static extern int QueryServiceStatus(IntPtr hService, ref ServiceStatus lpServiceStatus);
    }
}
