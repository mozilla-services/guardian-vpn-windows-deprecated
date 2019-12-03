// <copyright file="WireGuardTunnelExitCodes.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Service exit codes provided by WireGuard upon failure.
    /// </summary>
    public enum WireGuardTunnelExitCodes
    {
        /// <summary>
        /// No error.
        /// </summary>
        ErrorSuccess,

        /// <summary>
        /// Error when opening the ringlogger log file.
        /// </summary>
        ErrorRingloggerOpen,

        /// <summary>
        /// Error while loading the WireGuard configuration file from path.
        /// </summary>
        ErrorLoadConfiguration,

        /// <summary>
        /// Error while creating a WinTun device.
        /// </summary>
        ErrorCreateWintun,

        /// <summary>
        /// Error while listening on a named pipe.
        /// </summary>
        ErrorUAPIListen,

        /// <summary>
        /// Error while resolving DNS hostname endpoints.
        /// </summary>
        ErrorDNSLookup,

        /// <summary>
        /// Error while manipulating firewall rules.
        /// </summary>
        ErrorFirewall,

        /// <summary>
        /// Error while setting the device configuration.
        /// </summary>
        ErrorDeviceSetConfig,

        /// <summary>
        /// Error while binding sockets to default routes.
        /// </summary>
        ErrorBindSocketsToDefaultRoutes,

        /// <summary>
        /// Unable to set interface addresses, routes, dns, and/or interface settings.
        /// </summary>
        ErrorSetNetConfig,

        /// <summary>
        /// Error while determining current executable path.
        /// </summary>
        ErrorDetermineExecutablePath,

        /// <summary>
        /// Error while opening the NUL file.
        /// </summary>
        ErrorOpenNULFile,

        /// <summary>
        /// Error while attempting to track tunnels.
        /// </summary>
        ErrorTrackTunnels,

        /// <summary>
        /// Error while attempting to enumerate current sessions.
        /// </summary>
        ErrorEnumerateSessions,

        /// <summary>
        /// Error while dropping privileges.
        /// </summary>
        ErrorDropPrivileges,

        /// <summary>
        /// Windows internal error.
        /// </summary>
        ErrorWin32,
    }
}
