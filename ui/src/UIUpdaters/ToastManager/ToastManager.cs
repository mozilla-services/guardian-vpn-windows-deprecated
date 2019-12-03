// <copyright file="ToastManager.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Manages the scheduling and display of in-app toasts.
    /// </summary>
    internal class ToastManager
    {
        private Thread updater = null;
        private CancellationTokenSource updaterCancellationTokenSource;

        private PersistentToastList persistentToastList = new PersistentToastList();
        private EphemeralToastQueue ephemeralToastQueue = new EphemeralToastQueue();

        /// <summary>
        /// Gets or sets a value indicating whether the UI main window is active or not.
        /// </summary>
        public bool MainWindowActive
        {
            get
            {
                if (Application.Current == null)
                {
                    return false;
                }

                return Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.MainWindow;
                    return mainWindow != null && mainWindow.GetType() == typeof(UI.MainWindow);
                });
            }

            set
            {
                if (!value)
                {
                    persistentToastList.ClearAll();
                }
            }
        }

        /// <summary>
        /// Starts the thread that handles scheduling in-app toasts.
        /// </summary>
        public void StartThread()
        {
            if (updater != null && updater.IsAlive)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();

            updater = new Thread(() => ScheduleToasts(updaterCancellationTokenSource.Token))
            {
                IsBackground = true,
            };
            updater.Start();
        }

        /// <summary>
        /// Stops the thread that handles scheduling in-app toasts.
        /// </summary>
        public void StopThread()
        {
            updaterCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Adds the desired toast to the queue/list be scheduled for display.
        /// </summary>
        /// <param name="toast">Toast to be displayed.</param>
        public void Show(UI.Components.Toast.Toast toast)
        {
            switch (toast.Display)
            {
                case UI.Components.Toast.Display.Ephemeral:
                    ephemeralToastQueue.Add(toast);
                    break;

                case UI.Components.Toast.Display.Persistent:
                    persistentToastList.Add(toast);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Refreshes the textblock content with all toasts currently within all toast lists/queues.
        /// </summary>
        public void RefreshAllToasts()
        {
            persistentToastList.RefreshAllContent();
            ephemeralToastQueue.RefreshAllContent();
        }

        private void ScheduleToasts(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Display persistent toasts
                var persistentToastCount = 0;
                while (persistentToastCount < persistentToastList.Capacity)
                {
                    if (!MainWindowActive)
                    {
                        break;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var persistentToast = persistentToastList.ToastList.ElementAtOrDefault(persistentToastCount);
                        if (persistentToast != null)
                        {
                            persistentToast.AddToElement();
                        }
                    });

                    persistentToastCount++;
                }

                // Scheduling for ephemeral toasts
                while (ephemeralToastQueue.Size > 0)
                {
                    if (!MainWindowActive)
                    {
                        break;
                    }

                    var ephemeralToast = ephemeralToastQueue.Dequeue();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ephemeralToast.AddToElement();
                    });

                    cancellationToken.WaitHandle.WaitOne(ephemeralToast.Duration);
                }

                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            }
        }
    }
}
