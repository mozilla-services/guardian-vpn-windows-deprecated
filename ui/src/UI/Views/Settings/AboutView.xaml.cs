// <copyright file="AboutView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for AboutView.xaml.
    /// </summary>
    public partial class AboutView : UserControl
    {
        private UserControl parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutView"/> class.
        /// </summary>
        /// <param name="parentView">Parent view control of the <see cref="AboutView"/> instance.</param>
        public AboutView(UserControl parentView)
        {
            this.parentView = parentView;
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public string ApplicationVersion
        {
            get
            {
                return ProductConstants.GetVersion();
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(parentView, MainWindow.SlideDirection.Right);
        }

        private void Privacy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.PrivacyUrl);
        }

        private void Terms_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.TermsUrl);
        }

        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "ZIP Archive|*.zip",
                Title = "Export debug package",
            };

            if (MessageBox.Show("Thank you for debugging the Firefox Private Network VPN client!\n\nThis utility will export a ZIP file to a directory of your choosing." +
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

        private void ViewLog_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            LogWindow.ShowLog(new Point(mainWindow.Left + mainWindow.Width, mainWindow.Top));
        }
    }
}
