// <copyright file="BrokerService.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Broker service message handler with elevated privileges.
    /// </summary>
    public partial class BrokerService : ServiceBase
    {
        /// <summary>
        /// Gets a cancellation token source for the broker process.
        /// </summary>
        public static CancellationTokenSource BrokerServiceTokenSource { get; private set; }

        /// <inheritdoc/>
        protected override void OnStart(string[] args)
        {
            // Initialize the broker service cancellation token
            BrokerServiceTokenSource = new CancellationTokenSource();

            // Start the initial broker child process
            Broker.StartChildProcess();
        }

        /// <inheritdoc/>
        protected override void OnStop()
        {
            // Stop all spawned broker child processes
            Broker.StopAllChildProcesses();
        }
    }
}
