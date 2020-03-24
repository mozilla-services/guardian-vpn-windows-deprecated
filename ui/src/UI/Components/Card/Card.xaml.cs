// <copyright file="Card.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for Card.xaml.
    /// </summary>
    public partial class Card : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates that the VPN status is not unprotected, triggering the card background to change.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Models.ConnectionState), typeof(Card), new PropertyMetadata(OnStatusChangedCallBack));

        /// <summary>
        /// Indicates that the VPN status is not unprotected, triggering the card background to change.
        /// </summary>
        public static readonly DependencyProperty IsStableProperty = DependencyProperty.Register("IsStable", typeof(bool), typeof(Card), new PropertyMetadata(IsStableChangedCallBack));

        /// <summary>
        /// Indicates that the VPN status is protected, trigger the ripple animation to start.
        /// </summary>
        public static readonly DependencyProperty IsCardShownProperty = DependencyProperty.Register("IsCardShown", typeof(bool), typeof(Card), new PropertyMetadata(OnIsCardShownChangedCallBack));

        private bool active = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// </summary>
        public Card()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the VPN status is not unprotected, triggering the card background to change.
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
        /// Gets or sets a value indicating whether the tunnel is stable.
        /// </summary>
        public bool IsStable
        {
            get
            {
                return (bool)GetValue(IsStableProperty);
            }

            set
            {
                SetValue(IsStableProperty, value);
                OnPropertyChanged("IsStable");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Card is in view.
        /// </summary>
        public bool IsCardShown
        {
            get
            {
                return (bool)GetValue(IsCardShownProperty);
            }

            set
            {
                SetValue(IsCardShownProperty, value);
                OnPropertyChanged("IsCardShown");
            }
        }

        /// <summary>
        /// Reacts when the property of the element has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property which has been changed.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            if (propertyName == "IsCardShown" || propertyName == "IsStable")
            {
                SetRippleAnimation();
            }

            if (propertyName == "Status")
            {
                SetRippleAnimation();
                SetCardUI();
            }
        }

        private static void OnStatusChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Card c = sender as Card;
            if (c != null)
            {
                c.OnPropertyChanged("Status");
            }
        }

        private static void IsStableChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Card c = sender as Card;
            if (c != null)
            {
                c.OnPropertyChanged("IsStable");
            }
        }

        private static void OnIsCardShownChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Card c = sender as Card;
            if (c != null)
            {
                c.OnPropertyChanged("IsCardShown");
            }
        }

        private bool ShouldAnimateRipple()
        {
            return Status == Models.ConnectionState.Protected && IsCardShown && IsStable;
        }

        private void SetRippleAnimation()
        {
            if (ShouldAnimateRipple())
            {
                StartRippleAnimation();
            }
            else
            {
                StopRippleAnimation();
            }
        }

        private void StartRippleAnimation()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && mainWindow.GetType() == typeof(UI.MainWindow))
                {
                    RippleAnimation.Running = true;
                }
            });
        }

        private void StopRippleAnimation()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && mainWindow.GetType() == typeof(UI.MainWindow))
                {
                    RippleAnimation.Running = false;
                }
            });
        }

        private void SetCardUI()
        {
            switch (Status)
            {
                case Models.ConnectionState.Connecting:
                    AnimateActive();
                    break;
                case Models.ConnectionState.Protected:
                    AnimateActive();
                    break;
                case Models.ConnectionState.Disconnecting:
                    AnimateInactive();
                    break;
                case Models.ConnectionState.Unprotected:
                    AnimateInactive();
                    break;
                default:
                    break;
            }
        }

        private void AnimateActive()
        {
            if (!active)
            {
                CardBorder.BeginStoryboard(this.FindResource("MakeActive") as Storyboard);
                active = true;
            }
        }

        private void AnimateInactive()
        {
            if (active)
            {
                CardBorder.BeginStoryboard(this.FindResource("MakeInactive") as Storyboard);
                active = false;
            }
        }

        private void Card_Unloaded(object sender, RoutedEventArgs e)
        {
            StopRippleAnimation();
        }

        private void Card_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                SetRippleAnimation();
            }
            else
            {
                StopRippleAnimation();
            }
        }
    }
}
