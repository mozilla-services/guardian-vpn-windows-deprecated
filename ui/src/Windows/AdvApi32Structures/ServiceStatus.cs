// <copyright file="ServiceStatus.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Contains status information for a service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        /// <summary>
        /// The type of service.
        /// </summary>
        public int DwServiceType;

        /// <summary>
        /// The current state of the service.
        /// </summary>
        public ServiceState DwCurrentState;

        /// <summary>
        /// The control codes the service accepts and processes in its handler function.
        /// </summary>
        public int DwControlsAccepted;

        /// <summary>
        /// The error code the service uses to report an error that occurs when it is starting or stopping.
        /// </summary>
        public int DwWin32ExitCode;

        /// <summary>
        /// A service-specific error code that the service returns when an error occurs while the service is starting or stopping.
        /// </summary>
        public int DwServiceSpecificExitCode;

        /// <summary>
        /// The check-point value the service increments periodically to report its progress during a lengthy start, stop, pause, or continue operation.
        /// </summary>
        public int DwCheckPoint;

        /// <summary>
        /// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds.
        /// </summary>
        public int DwWaitHint;
    }
}
