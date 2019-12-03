// <copyright file="VPNToggle.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for VPNToggle.xaml.
    /// </summary>
    public partial class VPNToggle : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Dependency property for the tunnel status.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Models.ConnectionState), typeof(VPNToggle), new PropertyMetadata(Models.ConnectionState.Unprotected));

        /// <summary>
        /// Dependency property for a flag that indicates that the white theme is enabled for the hover border when the toggle is turned on.
        /// </summary>
        public static readonly DependencyProperty EnableWhiteSwitchBorderProperty = DependencyProperty.Register("EnableWhiteSwitchBorder", typeof(bool), typeof(VPNToggle), new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="VPNToggle"/> class.
        /// </summary>
        public VPNToggle()
        {
            InitializeComponent();
            DataContext = Manager.MainWindowViewModel;
            SetButtonUI(ButtonRefreshMode.WithoutAnimation);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private enum ButtonRefreshMode
        {
            WithAnimation,
            WithoutAnimation,
        }

        /// <summary>
        /// Gets or sets the tunnel status dependency property.
        /// </summary>
        public Models.ConnectionState Status
        {
            get
            {
                return (Models.ConnectionState)GetValue(StatusProperty);
            }

            set
            {
                SetValue(StatusProperty, value);
                OnPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the white theme is enabled for the hover border when the toggle is turned on.
        /// </summary>
        public bool EnableWhiteSwitchBorder
        {
            get
            {
                return (bool)GetValue(EnableWhiteSwitchBorderProperty);
            }

            set
            {
                SetValue(EnableWhiteSwitchBorderProperty, value);
            }
        }

        /// <summary>
        /// Reacts to dynamic changes to properties within this control.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            if (propertyName == "Status")
            {
                SetButtonUI();
            }
        }

        private void AnimateOn(ButtonRefreshMode mode = ButtonRefreshMode.WithAnimation)
        {
            Thickness newThickness = SphereContainer.Margin;
            if (newThickness.Left == 31)
            {
                return;
            }

            newThickness.Left = 31;

            if (mode == ButtonRefreshMode.WithAnimation)
            {
                SphereContainer.BeginAnimation(Border.MarginProperty,
                    new ThicknessAnimation(newThickness, TimeSpan.FromSeconds(0.15), System.Windows.Media.Animation.FillBehavior.HoldEnd)
                );
            }
            else
            {
                SphereContainer.Margin = newThickness;
            }

            SphereContainer.BeginStoryboard(this.FindResource("MakeGreenColor") as Storyboard);
            if (EnableWhiteSwitchBorder)
            {
                SwitchBorder.BorderBrush = this.FindResource("Grey/White 20") as Brush;
            }
        }

        private void AnimateOff(ButtonRefreshMode mode = ButtonRefreshMode.WithAnimation)
        {
            System.Windows.Thickness newThickness = SphereContainer.Margin;
            if (newThickness.Left == 5)
            {
                return;
            }

            newThickness.Left = 5;

            if (mode == ButtonRefreshMode.WithAnimation)
            {
                SphereContainer.BeginAnimation(Canvas.MarginProperty,
                    new ThicknessAnimation(newThickness, TimeSpan.FromSeconds(0.15), System.Windows.Media.Animation.FillBehavior.HoldEnd)
                );
            }
            else
            {
                SphereContainer.Margin = newThickness;
            }

            SphereContainer.BeginStoryboard(this.FindResource("MakeGrayColor") as Storyboard);
            if (EnableWhiteSwitchBorder)
            {
                SwitchBorder.BorderBrush = this.FindResource("Grey/Grey 10") as Brush;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Status == Models.ConnectionState.Protected)
            {
                Status = Models.ConnectionState.Disconnecting;
                return;
            }

            if (Status == Models.ConnectionState.Unprotected)
            {
                Status = Models.ConnectionState.Connecting;
                return;
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Status == Models.ConnectionState.Protected)
            {
                SphereContainer.BeginStoryboard(this.FindResource("MakeDarkGreenColor") as Storyboard);
                return;
            }

            if (Status == Models.ConnectionState.Unprotected)
            {
                SphereContainer.BeginStoryboard(this.FindResource("MakeDarkGrayColor") as Storyboard);
                return;
            }

            if (Status == Models.ConnectionState.Connecting)
            {
                SphereContainer.BeginStoryboard(this.FindResource("MakeDarkGreenColor") as Storyboard);
                return;
            }

            if (Status == Models.ConnectionState.Disconnecting)
            {
                SphereContainer.BeginStoryboard(this.FindResource("MakeDarkGrayColor") as Storyboard);
                return;
            }
        }

        private void SetButtonUI(ButtonRefreshMode mode = ButtonRefreshMode.WithAnimation)
        {
            if (Status == Models.ConnectionState.Protected)
            {
                AnimateOn(mode);
                this.IsEnabled = true;
            }

            if (Status == Models.ConnectionState.Connecting)
            {
                AnimateOn(mode);
                this.IsEnabled = false;
            }

            if (Status == Models.ConnectionState.Unprotected)
            {
                AnimateOff(mode);
                this.IsEnabled = true;
            }

            if (Status == Models.ConnectionState.Disconnecting)
            {
                AnimateOff(mode);
                this.IsEnabled = false;
            }
        }
    }
}
