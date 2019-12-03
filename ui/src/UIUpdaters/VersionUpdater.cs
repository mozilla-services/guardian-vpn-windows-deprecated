// <copyright file="VersionUpdater.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Periodically polls for application version updates and updates the UI accordingly.
    /// </summary>
    internal class VersionUpdater
    {
        private CancellationTokenSource updaterCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionUpdater"/> class.
        /// </summary>
        public VersionUpdater()
        {
        }

        /// <summary>
        /// Starts the version update polling task.
        /// </summary>
        public void StartTask()
        {
            if (updaterCancellationTokenSource != null && !updaterCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();
            UpdateVersionAsync(updaterCancellationTokenSource.Token);
        }

        /// <summary>
        /// Stops the version update polling task.
        /// </summary>
        public void StopTask()
        {
            updaterCancellationTokenSource.Cancel();
        }

        private async void UpdateVersionAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                JSONStructures.BalrogResponse balrogResponse;

                try
                {
                    balrogResponse = await Update.Balrog.QueryUpdate(ProductConstants.GetNumericVersion());
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException)
                    {
                        cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(5));
                        continue;
                    }
                    else
                    {
                        ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                        throw e;
                    }
                }

                if (balrogResponse != null)
                {
                    if (balrogResponse.Required)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var owner = Application.Current.MainWindow;
                            if (owner != null)
                            {
                                ((UI.MainWindow)owner).NavigateToView(new UI.UpdateView(), UI.MainWindow.SlideDirection.Right);
                                Manager.MustUpdate = true;
                            }
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (Manager.MainWindowViewModel.UpdateToast == null)
                            {
                                // Create an update toast
                                var message = new ErrorHandling.UserFacingMessage(
                                    "toast-update-version-message-1",
                                    new ErrorHandling.UserFacingMessage[] { new ErrorHandling.UserFacingMessage("toast-update-version-message-2", new List<Type>() { typeof(Underline), typeof(Bold) }) }
                                );

                                var toast = new UI.Components.Toast.Toast(UI.Components.Toast.Style.Info, message, display: UI.Components.Toast.Display.Persistent, priority: UI.Components.Toast.Priority.Important)
                                {
                                    ClickEventHandler = (sender, e) =>
                                    {
                                        // Create an update toast
                                        var updateToast = new UI.Components.Toast.Toast(UI.Components.Toast.Style.Info, new ErrorHandling.UserFacingMessage("update-update-started"), priority: UI.Components.Toast.Priority.Important);
                                        Manager.ToastManager.Show(updateToast);
                                        ErrorHandling.ErrorHandler.WriteToLog(Manager.TranslationService.GetString("update-update-started"), ErrorHandling.LogLevel.Info);

                                        var updateTask = Task.Run(async () =>
                                        {
                                            var success = await Update.Update.Run(ProductConstants.GetNumericVersion());
                                            if (success)
                                            {
                                                // Dismiss the update toast
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    Manager.MainWindowViewModel.UpdateToast.Toast_Dismiss(null, null);
                                                });
                                            }
                                            else
                                            {
                                                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("update-update-failed"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                                            }
                                        });
                                    },
                                };

                                Manager.MainWindowViewModel.UpdateToast = toast;
                            }

                            var owner = Application.Current.MainWindow;
                            if (owner != null)
                            {
                                // Show the update toast
                                Manager.ToastManager.Show(Manager.MainWindowViewModel.UpdateToast);
                            }
                        });
                    }
                }

                cancellationToken.WaitHandle.WaitOne(TimeSpan.FromHours(6));
            }
        }
    }
}
