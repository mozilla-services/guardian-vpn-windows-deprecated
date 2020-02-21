// <copyright file="MainView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for view1.xaml.
    /// </summary>
    public partial class MainView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;

            // Reinitialize UI values
            ReinitializeUI();
        }

        private void ReinitializeUI()
        {
            InitializeToggle();
            InitializeConnectionNavButton();

            Manager.AccountInfoUpdater.RefreshDeviceList();
        }

        private void InitializeToggle()
        {
            Toggle.Status = Manager.MainWindowViewModel.TunnelStatus;
        }

        private void InitializeConnectionNavButton()
        {
            if (Manager.MainWindowViewModel.ServerCityListSelectedItem == null)
            {
                return;
            }

            var selectedServer = FxA.Cache.FxAServerList.GetServerByIP(Manager.MainWindowViewModel.ServerSelected.Endpoint);

            if (selectedServer != null)
            {
                ConnectionNavButton.Subtitle = selectedServer.City;

                var flagIcon = Application.Current.TryFindResource(selectedServer.Country);

                if (flagIcon != null)
                {
                    ConnectionNavButton.SubtitleIconUrl = flagIcon.ToString();
                }
            }
        }

        private void NavigateConnection(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new ConnectionView(), MainWindow.SlideDirection.Left);
        }

        private void NavigateDevices(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new DevicesView(), MainWindow.SlideDirection.Left);
        }

        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new SettingsView(), MainWindow.SlideDirection.Up);
        }

        private void Toggle_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Toggle.Status = Manager.MainWindowViewModel.TunnelStatus;
        }
    }
}
