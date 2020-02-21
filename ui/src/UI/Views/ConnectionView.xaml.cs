// <copyright file="ConnectionView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FirefoxPrivateNetwork.FxA;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for view2.xaml.
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionView"/> class.
        /// </summary>
        public ConnectionView()
        {
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;

            var selectedServerIndex = FxA.Cache.FxAServerList.GetServerIndexByCountry(Manager.MainWindowViewModel.CountryServerList, Manager.MainWindowViewModel.ServerCityListSelectedItem.Country);
            CountryServerList.Loaded += (s, e) => CountryServerList.ScrollIntoView(Manager.MainWindowViewModel.CountryServerList[selectedServerIndex]);
        }

        private void NavigateMain(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new MainView(), MainWindow.SlideDirection.Right);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ((sender as RadioButton)?.Tag as ListViewItem)?.DataContext;

            if (selectedItem is Models.CityServerListItem selectedCity)
            {
                var previousSelectedCity = Manager.MainWindowViewModel.ServerCityListSelectedItem;

                // Set the selected server city and server
                Manager.MainWindowViewModel.ServerCityListSelectedItem = selectedCity;
                Manager.MainWindowViewModel.UpdateServerSelection();

                // Switch servers if presently connected, do nothing otherwise
                if (Manager.MainWindowViewModel.Status == Models.ConnectionState.Protected)
                {
                    WireGuard.Connector.Connect(switchServer: true, previousServerCity: previousSelectedCity.City, switchServerCity: selectedCity.City);
                }

                NavigateMain(sender, e);
            }
        }
    }
}
