// <copyright file="SettingsView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml.
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="SettingsView"/> instance.</param>
        public SettingsView(UserControl parentView = null)
        {
            this.parentView = parentView ?? new MainView();
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Gets the current culture info set by the OS.
        /// </summary>
        public string InstalledUICulture => CultureInfo.InstalledUICulture.ToString();

        /// <summary>
        /// Gets a value indicating whether the "Unsecure network alert" checkbox is checked.
        /// </summary>
        public bool UnsecureNetworkAlert => Manager.Settings.Network.UnsecureNetworkAlert;

        /// <summary>
        /// Gets a value indicating whether the "Allow local device access" checkbox is checked.
        /// </summary>
        public bool AllowLocalDeviceAccess => Manager.Settings.Network.AllowLocalDeviceAccess;

        /// <summary>
        /// Gets the user's name which will be displayed in the settings card.
        /// </summary>
        public string UserDisplayName => Manager.Account.Config.FxALogin.User.DisplayName;

        /// <summary>
        /// Gets the user's e-mail address.
        /// </summary>
        public string UserEmail => Manager.Account.Config.FxALogin.User.Email;

        /// <summary>
        /// Gets or sets a value indicating whether the application is set to run at startup.
        /// </summary>
        public bool RunOnStartup
        {
            get
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                return registryKey.GetValue(ProductConstants.ProductName) != null;
            }

            set
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (value)
                {
                    registryKey.SetValue(ProductConstants.ProductName, string.Concat(System.Reflection.Assembly.GetExecutingAssembly().Location, " -s"));
                }
                else
                {
                    registryKey.DeleteValue(ProductConstants.ProductName);
                }
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            parentView.DataContext = null;
            parentView.DataContext = Manager.MainWindowViewModel;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Down);
        }

        private void NavigateGetHelp(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new GetHelpView(this), MainWindow.SlideDirection.Left);
        }

        private void NavigateAbout(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new AboutView(this), MainWindow.SlideDirection.Left);
        }

        private void NavigateLanguage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new LanguageView(this), MainWindow.SlideDirection.Left);
        }

        private void RunOnStartup_Click(object sender, RoutedEventArgs e)
        {
            CheckBox runOnStartupCheckBox = sender as CheckBox;
            RunOnStartup = runOnStartupCheckBox.IsChecked ?? false;
        }

        private void UnsecureNetworkAlertCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox unsecureNetworkAlertCheckBox = sender as CheckBox;

            // Save the new Unsecured Network Alert settings
            var networkSettings = Manager.Settings.Network;
            networkSettings.UnsecureNetworkAlert = unsecureNetworkAlertCheckBox.IsChecked ?? false;
            Manager.Settings.Network = networkSettings;
        }

        private void AllowLocalDeviceAccessCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox allowLocalDeviceAccessCheckBox = sender as CheckBox;

            // Save the new Unsecured Network Alert settings
            var networkSettings = Manager.Settings.Network;
            networkSettings.AllowLocalDeviceAccess = allowLocalDeviceAccessCheckBox.IsChecked ?? false;
            Manager.Settings.Network = networkSettings;

            // Reconfigure the VPN allowed IPs
            ProductConstants.AllowedIPs = Manager.Settings.Network.AllowLocalDeviceAccess ? ProductConstants.DefaultAllowedIPsLocal : ProductConstants.DefaultAllowedIPs;
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.FeedbackFormUrl);
        }

        private void Signout_Click(object sender, RoutedEventArgs e)
        {
            Manager.Account.Logout(removeDevice: true);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new LandingView(), MainWindow.SlideDirection.Down);
        }

        private void ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.FxAAccountManagementUrl);
        }
    }
}