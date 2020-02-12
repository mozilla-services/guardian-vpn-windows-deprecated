// <copyright file="IpInfoUpdater.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using System.Windows;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Periodically retrieves the IP information of the client and updates the UI accordingly.
    /// </summary>
    internal class IpInfoUpdater
    {
        private Thread updater = null;
        private CancellationTokenSource updaterCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpInfoUpdater"/> class.
        /// </summary>
        public IpInfoUpdater()
        {
        }

        /// <summary>
        /// Starts the IP information retrieval thread.
        /// </summary>
        public void StartThread()
        {
            if (updater != null && updater.IsAlive)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();

            updater = new Thread(() => UpdateIpInfo(updaterCancellationTokenSource.Token))
            {
                IsBackground = true,
            };
            updater.Start();
        }

        /// <summary>
        /// Stops the IP information retrieval thread.
        /// </summary>
        public void StopThread()
        {
            updaterCancellationTokenSource.Cancel();
        }

        private void UpdateIpInfo(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Manager.Account.LoginState == FxA.LoginState.LoggedIn)
                {
                    var ipInfo = new FxA.IpInfo();
                    ipInfo.RetreiveIpInfo();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var owner = Application.Current.MainWindow;
                        if (owner != null)
                        {
                            ipInfo.RetreiveIpInfo();
                        }
                    });
                }

                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(ProductConstants.IpInfoRefreshPeriod));
            }
        }
    }
}
