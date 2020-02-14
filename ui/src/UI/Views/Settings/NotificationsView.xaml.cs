// <copyright file="NotificationsView.xaml.cs" company="Mozilla">
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
    /// Interaction logic for NotificationsView.xaml.
    /// </summary>
    public partial class NotificationsView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="NotificationsView"/> instance.</param>
        public NotificationsView(UserControl parentView)
        {
            this.parentView = parentView;
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;
        }

        /// <summary>
        /// Gets a value indicating whether the "Unsecure network alert" checkbox is checked.
        /// </summary>
        public bool UnsecureNetworkAlert => Manager.Settings.Network.UnsecureNetworkAlert;

        /// <summary>
        /// Gets a value indicating whether the "Captive portal alert" checkbox is checked.
        /// </summary>
        public bool CaptivePortalAlert => Manager.Settings.Network.CaptivePortalAlert;

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Right);
        }

        private void UnsecureNetworkAlertCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox unsecureNetworkAlertCheckBox = sender as CheckBox;

            // Save the new Unsecured Network Alert settings
            var networkSettings = Manager.Settings.Network;
            networkSettings.UnsecureNetworkAlert = unsecureNetworkAlertCheckBox.IsChecked ?? false;
            Manager.Settings.Network = networkSettings;
        }

        private void CaptivePortalAlertCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox captivePortalAlertCheckBox = sender as CheckBox;

            // Save the new Captive Portal Alert settings
            var networkSettings = Manager.Settings.Network;
            networkSettings.CaptivePortalAlert = captivePortalAlertCheckBox.IsChecked ?? false;
            Manager.Settings.Network = networkSettings;
        }
    }
}
