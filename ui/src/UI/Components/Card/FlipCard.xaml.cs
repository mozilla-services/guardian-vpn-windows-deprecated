// <copyright file="FlipCard.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for FlipCard.xaml.
    /// </summary>
    public partial class FlipCard : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Dependency property for the IP address for the selected server.
        /// </summary>
        public static readonly DependencyProperty IPAddressProperty = DependencyProperty.Register("IPAddress", typeof(string), typeof(FlipCard), new PropertyMetadata(IPAddressChangedCallBack));

        /// <summary>
        /// Initializes a new instance of the <see cref="FlipCard"/> class.
        /// </summary>
        public FlipCard()
        {
            DataContext = Manager.MainWindowViewModel;
            InitializeComponent();
            InitializeToggle();
            HasVpnConnectionSideUp = true;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the IP address property of the selected server.
        /// </summary>
        public string IPAddress
        {
            get
            {
                return (string)GetValue(IPAddressProperty);
            }

            set
            {
                SetValue(IPAddressProperty, value);
                OnPropertyChanged("IPAddress");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the VPN connection side of the card is shown.
        /// </summary>
        public bool HasVpnConnectionSideUp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the network usage side of the card is up and in view.
        /// </summary>
        public bool HasDataUsageSideUp
        {
            get;
            private set;
        }

        /// <summary>
        /// Flips the card to the other side.
        /// </summary>
        /// <param name="animated">Value indicating whether the flip should be animated.</param>
        public void Flip(bool animated)
        {
            if (VpnConnection.Visibility == Visibility.Visible)
            {
                FlipToDataUsage(animated);
            }
            else
            {
                FlipToVpnConnection(animated);
            }
        }

        /// <summary>
        /// Reacts when the property of the element has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property which has been changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private static void IPAddressChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FlipCard f = sender as FlipCard;
            if (f != null)
            {
                f.OnPropertyChanged("IPAddress");
            }
        }

        private void Flip(object sender, RoutedEventArgs e)
        {
            Flip(animated: true);
        }

        private void FlipToDataUsage(bool animated)
        {
            HasDataUsageSideUp = true;
            OnPropertyChanged("HasDataUsageSideUp");
            HasVpnConnectionSideUp = false;
            OnPropertyChanged("HasVpnConnectionSideUp");

            SpeedVisual.DrawCurves();

            if (animated)
            {
                Storyboard flipAnimation = Resources["FlipToDataUsageStoryboard"] as Storyboard;

                AddTimelineCompleteHandler(flipAnimation, () => UpdateCardVisibility());

                DataUsage.Visibility = Visibility.Visible;
                flipAnimation.Begin();
            }
            else
            {
                UpdateCardVisibility();
            }
        }

        private void FlipToVpnConnection(bool animated)
        {
            HasDataUsageSideUp = false;
            OnPropertyChanged("HasDataUsageSideUp");
            HasVpnConnectionSideUp = true;
            OnPropertyChanged("HasVpnConnectionSideUp");

            if (animated)
            {
                Storyboard flipAnimation = Resources["FlipToVpnConnectionStoryboard"] as Storyboard;

                AddTimelineCompleteHandler(flipAnimation, () => UpdateCardVisibility());

                VpnConnection.Visibility = Visibility.Visible;
                flipAnimation.Begin();
            }
            else
            {
                UpdateCardVisibility();
            }
        }

        private void UpdateCardVisibility()
        {
            if (HasDataUsageSideUp)
            {
                VpnConnection.Visibility = Visibility.Collapsed;
                DataUsage.Visibility = Visibility.Visible;
            }
            else
            {
                VpnConnection.Visibility = Visibility.Visible;
                DataUsage.Visibility = Visibility.Collapsed;
            }
        }

        private void AddTimelineCompleteHandler(Timeline timeline, Action action)
        {
            EventHandler completeHandler = null;
            completeHandler = (sender, e) =>
            {
                timeline.Completed -= completeHandler;
                action();
            };
            timeline.Completed += completeHandler;
        }

        private void InitializeToggle()
        {
            Toggle.Status = Manager.MainWindowViewModel.TunnelStatus;
        }

        private void Toggle_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            Toggle.Status = Manager.MainWindowViewModel.TunnelStatus;
        }

        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToView(new SettingsView(Manager.MainWindowViewModel.ViewFrameSource), MainWindow.SlideDirection.Up);
        }
    }
}
