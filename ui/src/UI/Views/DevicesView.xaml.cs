// <copyright file="DevicesView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for view3.xaml.
    /// </summary>
    public partial class DevicesView : UserControl
    {
        /// <summary>
        /// Dependency property for a flag indicating whether the user's device limit has been reached or not.
        /// </summary>
        public static readonly DependencyProperty DeviceLimitReachedProperty = DependencyProperty.Register("DeviceLimitReached", typeof(bool), typeof(DevicesView), new PropertyMetadata(false));

        private string fxaJson;
        private Button currentDeleteButton = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesView"/> class.
        /// </summary>
        /// <param name="deviceLimitReached">A flag that indicates whether the user's device limit has been reached or not.</param>
        /// <param name="fxaJson">JSON string received from FxA containing login data.</param>
        public DevicesView(bool deviceLimitReached = false, string fxaJson = "")
        {
            DeviceLimitReached = deviceLimitReached;
            this.fxaJson = fxaJson;

            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;

            Manager.AccountInfoUpdater.RefreshDeviceList();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user's device limit has been reached or not.
        /// </summary>
        public bool DeviceLimitReached
        {
            get
            {
                return (bool)GetValue(DeviceLimitReachedProperty);
            }

            set
            {
                SetValue(DeviceLimitReachedProperty, value);
            }
        }

        /// <summary>
        /// Gets the devices page description.
        /// </summary>
        public string DevicesPageDescription => Manager.TranslationService.GetString("devices-page-description", UI.Resources.Localization.TranslationService.Args("numMaxDevices", Manager.MainWindowViewModel.MaxNumDevices));

        /// <summary>
        /// Navigates to the Main view.
        /// </summary>
        private void NavigateMain(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new MainView(), MainWindow.SlideDirection.Right);
        }

        /// <summary>
        /// Navigates to the Settings View.
        /// </summary>
        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new SettingsView(this), MainWindow.SlideDirection.Up);
        }

        /// <summary>
        /// Event handler for when a device list item delete button is clicked.
        /// </summary>
        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            InitializeDeleteDevicePopup();

            Button deleteDeviceButton = sender as Button;
            Models.DeviceListItem device = deleteDeviceButton.DataContext as Models.DeviceListItem;
            DeleteDevices.DeviceToDelete = device;

            DeleteDevices.PopupTitle = Manager.TranslationService.GetString("devices-remove-popup-title");
            DeleteDevices.PopupContent = Manager.TranslationService.GetString("devices-remove-popup-content", UI.Resources.Localization.TranslationService.Args("deviceName", device.Name));

            OpenDeleteDevicePopup();

            ButtonExtensions.SetMarkForDeletion(deleteDeviceButton, true);
            DisableDeleteDeviceButton(deleteDeviceButton);
            currentDeleteButton = deleteDeviceButton;
        }

        /// <summary>
        /// Initializes the popup for device removal by attaching event handlers to corresponding popup buttons.
        /// </summary>
        private void InitializeDeleteDevicePopup()
        {
            DeleteDevices.CancelButton = true;
            DeleteDevices.CancelButtonEventHandler = (sender, e) => { DeleteDevicePopup_Cancel(sender, e); };

            DeleteDevices.RemoveButton = true;
            DeleteDevices.RemoveButtonEventHandler = (sender, e) => { DeleteDevicePopup_Remove(sender, e); };

            DeleteDevices.PopupClosedEventHandler = (sender, e) => { DeleteDevicePopup_Cancel(sender, e); };
        }

        /// <summary>
        /// Behaviour for when the cancel button in the device removal popup is clicked.
        /// </summary>
        private void DeleteDevicePopup_Cancel(object sender, EventArgs e)
        {
            // Cancel device removal flags and re-enable delete button if not currently deleting
            if (currentDeleteButton != null && !ButtonExtensions.GetDeleting(currentDeleteButton))
            {
                ButtonExtensions.SetMarkForDeletion(currentDeleteButton, false);
                EnableDeleteDeviceButton(currentDeleteButton);
                currentDeleteButton = null;
            }

            CloseDeleteDevicePopup();
        }

        /// <summary>
        /// Behaviour for when the remove button in the device removal popup is clicked.
        /// </summary>
        private void DeleteDevicePopup_Remove(object sender, EventArgs e)
        {
            // Set delete button state to deleting
            ButtonExtensions.SetDeleting(currentDeleteButton, true);
            var deleteButton = currentDeleteButton;

            // Initiate the async device removal task
            var devices = new FxA.Devices();
            var removeDeviceTask = devices.RemoveDeviceTask(DeleteDevices.DeviceToDelete.Pubkey, safeRemove: true, silent: true);

            // Check if device removal task was successful
            removeDeviceTask.ContinueWith(task =>
            {
                if (!task.Result)
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-remove-device-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ButtonExtensions.SetMarkForDeletion(deleteButton, false);
                        ButtonExtensions.SetDeleting(deleteButton, false);
                        EnableDeleteDeviceButton(deleteButton);
                    });
                }
            });

            // If on the device limit reached page, reprocess the login response upon device removal
            if (DeviceLimitReached && !string.IsNullOrEmpty(fxaJson))
            {
                removeDeviceTask.ContinueWith(task =>
                {
                    if (task.Result)
                    {
                        var reprocessLoginResult = Manager.Account.ProcessLogin(fxaJson);
                        if (reprocessLoginResult)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                var owner = Application.Current.MainWindow;
                                if (owner != null)
                                {
                                    ((UI.MainWindow)owner).NavigateToView(new UI.OnboardingView5(), UI.MainWindow.SlideDirection.Left);
                                    ((UI.MainWindow)owner).Activate();
                                }
                            });
                        }
                    }
                });
            }

            // Close the delete device popup
            DeleteDevicePopup_Cancel(sender, e);
        }

        /// <summary>
        /// Disables the delete device button and reduces its opacity.
        /// </summary>
        private void DisableDeleteDeviceButton(Button deleteDeviceButton)
        {
            deleteDeviceButton.Opacity = 0.5;
            deleteDeviceButton.IsEnabled = false;
        }

        /// <summary>
        /// Enables the delete device button and resets its opacity.
        /// </summary>
        private void EnableDeleteDeviceButton(Button deleteDeviceButton)
        {
            deleteDeviceButton.Opacity = 1;
            deleteDeviceButton.IsEnabled = true;
        }

        /// <summary>
        /// Opens the delete device popup.
        /// </summary>
        private void OpenDeleteDevicePopup()
        {
            var deleteDevicesPopup = DeleteDevices.FindName("DeleteDevicesPopup") as Popup;
            deleteDevicesPopup.IsOpen = true;
        }

        /// <summary>
        /// Closes the delete device popup.
        /// </summary>
        private void CloseDeleteDevicePopup()
        {
            var deleteDevicesPopup = DeleteDevices.FindName("DeleteDevicesPopup") as Popup;
            deleteDevicesPopup.IsOpen = false;
        }
    }
}
