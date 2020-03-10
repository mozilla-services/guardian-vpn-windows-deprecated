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
        private EventWaitHandle forcedUpdateHandle;
        private bool forceUpdatePending = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpInfoUpdater"/> class.
        /// </summary>
        public IpInfoUpdater()
        {
            forcedUpdateHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
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

            updater = new Thread(() => UpdateIpInfo())
            {
                IsBackground = true,
            };
            updater.Start();
        }

        /// <summary>
        /// Forces an update of the public IP address and any associated info.
        /// </summary>
        public void ForceUpdate()
        {
            forcedUpdateHandle.Set();
            forceUpdatePending = true;
        }

        /// <summary>
        /// Stops the IP information retrieval thread.
        /// </summary>
        public void StopThread()
        {
            updaterCancellationTokenSource.Cancel();
        }

        private void UpdateIpInfo()
        {
            while (!updaterCancellationTokenSource.Token.IsCancellationRequested)
            {
                forcedUpdateHandle.Reset();

                if (Manager.Account.LoginState == FxA.LoginState.LoggedIn)
                {
                    var maxRetries = 1;

                    // Attempt multiple retries if a force update is pending.
                    // It may take more time for the client to use the new primary connection when connected/disconnected.
                    if (forceUpdatePending)
                    {
                        maxRetries = ProductConstants.IpInfoRefreshGraceRetries;
                        forceUpdatePending = false;
                    }

                    for (var retry = 0; retry < maxRetries; retry++)
                    {
                        updaterCancellationTokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(ProductConstants.IpInfoRefreshGracePeriod));
                        var ipInfo = FxA.IpInfo.RetrieveIpInfo();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var owner = Application.Current.MainWindow;
                            if (owner != null)
                            {
                                if (ipInfo != null)
                                {
                                    Manager.MainWindowViewModel.IpAddressString = "IP: " + ipInfo.Ip;
                                }
                            }
                        });
                    }
                }

                var waitHandles = new WaitHandle[] { updaterCancellationTokenSource.Token.WaitHandle, forcedUpdateHandle };
                WaitHandle.WaitAny(waitHandles, TimeSpan.FromMinutes(ProductConstants.IpInfoRefreshPeriod));
            }
        }
    }
}
