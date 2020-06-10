// <copyright file="VerifyAccountView.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for VerifyAccountView.xaml.
    /// </summary>
    public partial class VerifyAccountView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyAccountView"/> class.
        /// </summary>
        /// <param name="loginURL">FxA login URL.</param>
        public VerifyAccountView(string loginURL)
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
        }

        private void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ProductConstants.SubscriptionUrl);
        }

        private void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            Manager.LoginSessionManager.StartNewSession();
        }
    }
}
