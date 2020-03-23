// <copyright file="SettingsView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
                if (registryKey.GetValue(ProductConstants.ProductName) == null)
                {
                    return false;
                }
                else
                {
                    return registryKey.GetValue(ProductConstants.ProductName).ToString().Contains(" -s");
                }
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
                    string productName = registryKey.GetValue(ProductConstants.ProductName).ToString();
                    if (productName.Contains(" -s") && productName.Contains(" -c"))
                    {
                        productName = productName.Remove(productName.IndexOf(" -s"), 3);
                        registryKey.SetValue(ProductConstants.ProductName, productName);
                    }
                    else if (productName.Contains(" -s"))
                    {
                        registryKey.DeleteValue(ProductConstants.ProductName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is set to connect at app launch.
        /// </summary>
        public bool ConnectOnStartup
        {
            get
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (registryKey.GetValue(ProductConstants.ProductName) == null)
                {
                    return false;
                }
                else
                {
                    return registryKey.GetValue(ProductConstants.ProductName).ToString().Contains(" -c");
                }
            }

            set
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (value)
                {
                    string currentProductName = registryKey.GetValue(ProductConstants.ProductName) == null ? System.Reflection.Assembly.GetExecutingAssembly().Location : registryKey.GetValue(ProductConstants.ProductName).ToString();
                    registryKey.SetValue(ProductConstants.ProductName, string.Concat(currentProductName, " -c"));
                }
                else
                {
                    // THIS ISN'T GOOD ENOUGH! IF ITS THE ONLY FLAG THEN DO REGISTRY.DELETEVALUE()...
                    if (registryKey.GetValue(ProductConstants.ProductName) != null)
                    {
                        string productName = registryKey.GetValue(ProductConstants.ProductName).ToString();

                        if (productName.Contains(" -s") && productName.Contains(" -c"))
                        {
                            productName = productName.Remove(productName.IndexOf(" -c"), 3);
                            registryKey.SetValue(ProductConstants.ProductName, productName);
                        }
                    }
                    else
                    {
                        registryKey.DeleteValue(ProductConstants.ProductName);
                    }
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

        private void NavigateNotifications(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new NotificationsView(this), MainWindow.SlideDirection.Left);
        }

        private void NavigateNetworkSettings(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new NetworkSettingsView(this), MainWindow.SlideDirection.Left);
        }

        private void RunOnStartup_Click(object sender, RoutedEventArgs e)
        {
            CheckBox runOnStartupCheckBox = sender as CheckBox;
            RunOnStartup = runOnStartupCheckBox.IsChecked ?? false;
        }

        private void ConnectOnStartup_Click(object sender, RoutedEventArgs e)
        {
            CheckBox connectOnLaunchCheckBox = sender as CheckBox;
            ConnectOnStartup = connectOnLaunchCheckBox.IsChecked ?? false;
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