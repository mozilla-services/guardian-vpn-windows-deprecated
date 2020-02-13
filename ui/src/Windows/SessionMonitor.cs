// <copyright file="SessionMonitor.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Monitor class for the current Windows user session.
    /// </summary>
    internal class SessionMonitor
    {
        private SessionSwitchEventHandler ssEventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionMonitor"/> class.
        /// </summary>
        /// <param name="ssEvent">Session switch event.</param>
        public SessionMonitor(SessionSwitchEvent ssEvent)
        {
            ssEventHandler = new SessionSwitchEventHandler(ssEvent);
        }

        /// <summary>
        /// Function delegate for the session switch event handler.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Session switch event arguments.</param>
        public delegate void SessionSwitchEvent(object sender, SessionSwitchEventArgs args);

        /// <summary>
        /// Set up the session switch event; handle incoming events.
        /// </summary>
        public void StartMonitor()
        {
            SystemEvents.SessionSwitch += ssEventHandler;
        }

        /// <summary>
        /// Removes the session switch event handler.
        /// </summary>
        public void StopMonitor()
        {
            SystemEvents.SessionSwitch -= ssEventHandler;
        }
    }
}
