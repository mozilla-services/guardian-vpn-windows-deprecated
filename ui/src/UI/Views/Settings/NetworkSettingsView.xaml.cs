// <copyright file="NetworkSettingsView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for NetworkSettingsView.xaml.
    /// </summary>
    public partial class NetworkSettingsView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSettingsView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="NetworkSettingsView"/> instance.</param>
        public NetworkSettingsView(UserControl parentView)
        {
            this.parentView = parentView;
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;
        }

        /// <summary>
        /// Gets a value indicating whether the "Enable IPv6" checkbox is checked.
        /// </summary>
        public bool EnableIPv6 => Manager.Settings.Network.EnableIPv6;

        /// <summary>
        /// Gets a value indicating whether the "Allow local device access" checkbox is checked.
        /// </summary>
        public bool AllowLocalDeviceAccess => Manager.Settings.Network.AllowLocalDeviceAccess;

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Right);
        }

        private void EnableIPv6CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox enableIPv6CheckBox = sender as CheckBox;

            // Save the new IPv6 settings
            var networkSettings = Manager.Settings.Network;
            networkSettings.EnableIPv6 = enableIPv6CheckBox.IsChecked ?? false;
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
    }
}
