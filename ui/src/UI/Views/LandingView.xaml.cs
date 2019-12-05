// <copyright file="LandingView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for LandingView.xaml.
    /// </summary>
    public partial class LandingView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LandingView"/> class.
        /// </summary>
        public LandingView()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        private void Signin_Click(object sender, RoutedEventArgs e)
        {
            var fxaLoginThread = new FxA.Login();
            fxaLoginThread.StartLogin();
        }

        private void NavigateOnboarding1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new OnboardingView1(), MainWindow.SlideDirection.Up);
        }
    }
}
