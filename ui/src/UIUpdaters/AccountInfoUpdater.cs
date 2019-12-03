// <copyright file="AccountInfoUpdater.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using NodaTime;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Polls account info from FxA API and reflect changes in UI.
    /// </summary>
    public class AccountInfoUpdater
    {
        private const int MaxPollWait = 3600; // Maximum time(sec) to wait before repolling
        private const int MinPollWait = 1; // Minimum time(sec) to wait before repolling
        private readonly ViewModels.MainWindowViewModel viewModel;

        private readonly HashSet<PollAccountInfoFlag> pollAccountInfoFlags = new HashSet<PollAccountInfoFlag>(); // Flags to indicate a force poll

        private int elapsedPollWait = 0; // Elapsed time since last poll
        private TaskCompletionSource<bool> pollAccountInfoComplete = new TaskCompletionSource<bool>(); // Indicates completion of the async GetAccountDetailsTask

        private Task updater = null; // Long running task / thread
        private CancellationTokenSource updaterCancellationTokenSource; // Indicates graceful exit from thread

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoUpdater"/> class.
        /// </summary>
        /// <param name="vm">MainWindowViewModel reference.</param>
        public AccountInfoUpdater(ViewModels.MainWindowViewModel vm)
        {
            viewModel = vm;
        }

        [Flags]
        private enum PollAccountInfoFlag
        {
            None,
            ForcePoll,
        }

        /// <summary>
        /// Starts the FxA account info polling task.
        /// </summary>
        public void StartTask()
        {
            if (updater != null && updater.Status == TaskStatus.Running)
            {
                return;
            }

            updaterCancellationTokenSource = new CancellationTokenSource();

            updater = Task.Factory.StartNew(() =>
            {
                PollAccountInfoAsync(updaterCancellationTokenSource.Token);
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops the FxA account info polling task.
        /// </summary>
        public void StopTask()
        {
            updaterCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Forces an account poll from FxA, disrupting the poll wait period.
        /// </summary>
        /// <returns>The force poll account info task.</returns>
        public Task ForcePollAccountInfo()
        {
            var forcePollTask = Task.Run(() =>
            {
                pollAccountInfoComplete = new TaskCompletionSource<bool>();
                pollAccountInfoFlags.Add(PollAccountInfoFlag.ForcePoll);

                // The attempt to get the Task.Result blocks until the completion source gets signaled.
                if (pollAccountInfoComplete.Task.Result)
                {
                    pollAccountInfoFlags.Remove(PollAccountInfoFlag.ForcePoll);
                }
            });

            return forcePollTask;
        }

        /// <summary>
        /// Refreshes the user's device list by forcing an account poll, then updates the view model data bindings related to user devices.
        /// </summary>
        public void RefreshDeviceList()
        {
            // Force an account poll and update the device list UI once the poll is complete
            ForcePollAccountInfo().ContinueWith(task =>
            {
                UpdateDeviceListUI(Manager.Account.Config.FxALogin.User.Devices);
            });
        }

        private async void PollAccountInfoAsync(CancellationToken cancellationToken)
        {
            int pollWaitLimit = MinPollWait;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Increment elapsed poll wait time
                elapsedPollWait += MinPollWait;

                // Poll for account details if the poll wait limit has been reached or a force poll flag has been indicated
                if (elapsedPollWait >= pollWaitLimit || pollAccountInfoFlags.Contains(PollAccountInfoFlag.ForcePoll))
                {
                    // Only poll for account details if user is logged in
                    if (Manager.Account.LoginState == FxA.LoginState.LoggedIn)
                    {
                        // Wait for fetching account details to be complete
                        Manager.Account.GetAccountDetailsTask(pollAccountInfoComplete).Wait();
                        if (Manager.Account.Config.FxALogin == null)
                        {
                            ErrorHandling.DebugLogger.LogDebugMsg("Get account details not complete");
                            await Task.Delay(TimeSpan.FromSeconds(MinPollWait));
                            continue;
                        }

                        // Get the updated user
                        var updatedUser = Manager.Account.Config.FxALogin.User;

                        // Redirect user to sign in view if user has been logged out
                        if (Manager.Account.LoginState == FxA.LoginState.LoggedOut)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                var owner = Application.Current.MainWindow;
                                if (owner != null)
                                {
                                    ((UI.MainWindow)owner).NavigateToView(new UI.LinkAccountView(), UI.MainWindow.SlideDirection.Right);
                                }
                            });
                        }

                        // Update subscription status
                        UpdateSubscriptionStatusUI(updatedUser.Subscriptions.Vpn.Active);

                        // Reset the poll wait limit to the maximum value
                        pollWaitLimit = MaxPollWait;
                    }
                    else
                    {
                        // Reset the poll wait limit to the minimum value until the user is logged in
                        pollWaitLimit = MinPollWait;
                    }

                    // Reset the elapsed poll wait time after account info has been polled
                    elapsedPollWait = 0;
                }

                await Task.Delay(TimeSpan.FromSeconds(MinPollWait));
            }
        }

        private void UpdateDeviceListUI(List<JSONStructures.Device> newDevices)
        {
            if (newDevices == null)
            {
                return;
            }

            var currentDevicePubKey = Manager.Account.Config.FxALogin.PublicKey;
            var newDeviceList = new List<Models.DeviceListItem>();

            foreach (var device in newDevices)
            {
                var localCreatedDate = device.CreatedAt.ToLocalTime();
                var dateAdded = GetDateAdded(device.CreatedAt);
                var newDevice = new Models.DeviceListItem
                {
                    Name = device.Name,
                    Pubkey = device.PublicKey,
                    Created = string.Format("{0} ({1})", dateAdded, localCreatedDate),
                    Added = (device.PublicKey == currentDevicePubKey) ? Manager.TranslationService.GetString("devices-current-device") : dateAdded,
                    CurrentDevice = (device.PublicKey == currentDevicePubKey) ? true : false,
                };

                if (newDevice.CurrentDevice)
                {
                    newDeviceList.Insert(0, newDevice);
                }
                else
                {
                    newDeviceList.Add(newDevice);
                }
            }

            Manager.MainWindowViewModel.DeviceList = newDeviceList;
            Manager.MainWindowViewModel.UserNumDevices = newDeviceList.Count();
            Manager.MainWindowViewModel.MaxNumDevices = Manager.Account.Config.FxALogin.User.MaxDevices;
        }

        private string GetDateAdded(DateTime localDate)
        {
            if (localDate.Date == DateTime.UtcNow.Date)
            {
                return Manager.TranslationService.GetString("devices-add-date-days", UI.Resources.Localization.TranslationService.Args("numDays", 0));
            }

            var period = Period.Between(new LocalDate(localDate.Year, localDate.Month, localDate.Day), new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            if (period.Years > 0)
            {
                return Manager.TranslationService.GetString("devices-add-date-years", UI.Resources.Localization.TranslationService.Args("numYears", period.Years));
            }
            else if (period.Months > 0)
            {
                return Manager.TranslationService.GetString("devices-add-date-months", UI.Resources.Localization.TranslationService.Args("numMonths", period.Months));
            }
            else
            {
                return Manager.TranslationService.GetString("devices-add-date-days", UI.Resources.Localization.TranslationService.Args("numDays", period.Days));
            }
        }

        private void UpdateSubscriptionStatusUI(bool newStatus)
        {
            if (newStatus)
            {
                viewModel.SubscriptionStatus = string.Format("Subscription: {0}", JSONStructures.SubscriptionStatus.Active);
            }
            else
            {
                viewModel.SubscriptionStatus = string.Format("Subscription: {0}", JSONStructures.SubscriptionStatus.Inactive);

                // Log the user out when their subscription is inactive
                Manager.Account.Logout(removeDevice: false);

                // Navigate the user to the sign in screen and display subscription expired toast
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var owner = Application.Current.MainWindow;
                    if (owner != null)
                    {
                        ((UI.MainWindow)owner).NavigateToView(new UI.LinkAccountView(), UI.MainWindow.SlideDirection.Right);

                        // Create a subscription expired toast
                        var message = new ErrorHandling.UserFacingMessage(
                            "toast-no-subscription",
                            new ErrorHandling.UserFacingMessage[] { new ErrorHandling.UserFacingMessage("subscribe-url-title", new List<Type>() { typeof(Underline), typeof(Bold) }) }
                        );

                        var toast = new UI.Components.Toast.Toast(UI.Components.Toast.Style.Info, message, priority: UI.Components.Toast.Priority.Important)
                        {
                            ClickEventHandler = (sender, e) =>
                            {
                                Process.Start(ProductConstants.SubscriptionUrl);
                            },
                        };

                        // Display the subscription expired toast
                        Manager.ToastManager.Show(toast);
                    }
                });
            }
        }
    }
}
