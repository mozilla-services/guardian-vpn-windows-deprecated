// <copyright file="GetHelpView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for GetHelpView.xaml.
    /// </summary>
    public partial class GetHelpView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetHelpView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="GetHelpView"/> instance.</param>
        public GetHelpView(UserControl parentView)
        {
            this.parentView = parentView;
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Right);
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.SupportUrl);
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.ContactUrl);
        }
    }
}
