// <copyright file="ServerListUpdater.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using System.Windows;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Periodically retrieves the VPN server list from the FxA API and updates the UI accordingly.
    /// </summary>
    internal class ServerListUpdater
    {
        private Thread updater = null;
        private CancellationTokenSource updaterCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerListUpdater"/> class.
        /// </summary>
        public ServerListUpdater()
        {
        }

        /// <summary>
        /// Starts the VPN server list retrieval thread.
        /// </summary>
        public void StartThread()
        {
            if (updater != null && updater.IsAlive)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();

            updater = new Thread(() => UpdateServerList(updaterCancellationTokenSource.Token))
            {
                IsBackground = true,
            };
            updater.Start();
        }

        /// <summary>
        /// Stops the VPN server list retrieval thread.
        /// </summary>
        public void StopThread()
        {
            updaterCancellationTokenSource.Cancel();
        }

        private void UpdateServerList(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Manager.Account.LoginState == FxA.LoginState.LoggedIn)
                {
                    FxA.Cache.FxAServerList.RetrieveRemoteServerList();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var owner = Application.Current.MainWindow;
                        if (owner != null)
                        {
                            ((UI.MainWindow)owner).RefreshServers();
                        }
                    });

                    cancellationToken.WaitHandle.WaitOne(TimeSpan.FromHours(1));
                }

                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }
    }
}
