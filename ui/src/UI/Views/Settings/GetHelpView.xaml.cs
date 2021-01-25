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

            if (parentView is LandingView)
            {
                // Since contact feature require user to have account first, we hide this item
                // if the user enter this page from the login screen.
                contactItem.Visibility = System.Windows.Visibility.Collapsed;
            }
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

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "ZIP Archive|*.zip",
                Title = "Export debug package",
            };

            if (MessageBox.Show("Thank you for debugging the Mozilla VPN client!\n\nThis utility will export a ZIP file to a directory of your choosing." +
                " This file will contain the following:\n\n- A list of your running processes\n- A list of your devices and device drivers\n" +
                "- Information about your network interfaces\n- Your computer hardware information\n\n" +
                "Along with the VPN tunnel log, the currently available list of VPN servers will also be included.\n" +
                "Your Firefox account information and any of your VPN credentials will not be exported.\n\n" +
                "Do you wish to proceed?",
                "Privacy notice",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.Yes)
            {
                saveDialog.ShowDialog();

                if (saveDialog.FileName != string.Empty)
                {
                    ErrorHandling.DebugDump.CreateDump(saveDialog.FileName);
                }
            }
        }
    }
}
