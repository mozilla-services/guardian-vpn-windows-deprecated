// <copyright file="IPCCommand.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Type of IPC command message.
    /// </summary>
    public struct IPCCommand
    {
        /// <summary>
        /// Connect command.
        /// </summary>
        public const string IpcConnect = "ipcconnect=1";

        /// <summary>
        /// Connect reply command.
        /// </summary>
        public const string IpcConnectReply = "ipcconnectreply=1";

        /// <summary>
        /// Disconnect command.
        /// </summary>
        public const string IpcDisconnect = "ipcdisconnect=1";

        /// <summary>
        /// Captive portal detection request.
        /// </summary>
        public const string IpcDetectCaptivePortal = "ipcdetectcaptiveportal=1";

        /// <summary>
        /// Captive portal detection reply.
        /// </summary>
        public const string IpcDetectCaptivePortalReply = "ipcdetectcaptiveportalreply=1";

        /// <summary>
        /// Request tunnel connection status.
        /// </summary>
        public const string IpcConnectionStatus = "ipcconnectionstatus=1";

        /// <summary>
        /// Tunnel connection status reply.
        /// </summary>
        public const string IpcConnectionStatusReply = "ipcconnectionstatusreply=1";

        /// <summary>
        /// Unknown IPC command.
        /// </summary>
        public const string IpcUnknown = "ipcunknown=1";

        /// <summary>
        /// WireGuard GET command.
        /// </summary>
        public const string WgGet = "get=1";

        /// <summary>
        /// WireGuard SET command.
        /// </summary>
        public const string WgSet = "set=1";
    }
}
