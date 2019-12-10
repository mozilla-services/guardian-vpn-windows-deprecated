// <copyright file="OnboardingView1.xaml.cs" company="Mozilla">
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
    /// Interaction logic for OnboardingView1.xaml.
    /// </summary>
    public partial class OnboardingView1 : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingView1"/> class.
        /// </summary>
        public OnboardingView1()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        private void NavigateOnboarding2(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new OnboardingView2(), MainWindow.SlideDirection.Left);
        }

        private void ExitOnboarding(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new LandingView(), MainWindow.SlideDirection.Down);
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            var fxaLoginThread = new FxA.Login();
            fxaLoginThread.StartLogin();
        }
    }
}
