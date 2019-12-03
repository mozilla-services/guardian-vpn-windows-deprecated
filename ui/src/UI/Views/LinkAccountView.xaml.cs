// <copyright file="LinkAccountView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for LinkAccount.xaml.
    /// </summary>
    public partial class LinkAccountView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkAccountView"/> class.
        /// </summary>
        public LinkAccountView()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        private void Signin_Click(object sender, RoutedEventArgs e)
        {
            var fxaLoginThread = new FxA.Login();
            fxaLoginThread.StartLogin();
        }

        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.SubscriptionUrl);
        }
    }
}
