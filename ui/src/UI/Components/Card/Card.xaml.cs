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
using System.Windows.Media.Animation;
using LottieSharp;

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
        /// Indicates that the VPN status is protected, trigger the ripple animation to start.
        /// </summary>
        public static readonly DependencyProperty AnimateRippleProperty = DependencyProperty.Register("AnimateRipple", typeof(bool), typeof(Card), new PropertyMetadata(OnAnimateRippleChangedCallBack));

        private bool active = false;
        private LottieAnimationView rippleAnimation = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// </summary>
        public Card()
        {
            ConstructRippleAnimation();
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
        /// Gets or sets a value indicating whether the VPN status is protected, trigger the ripple animation to start.
        /// </summary>
        public bool AnimateRipple
        {
            get
            {
                return (bool)GetValue(AnimateRippleProperty);
            }

            set
            {
                SetValue(AnimateRippleProperty, value);
                OnPropertyChanged("AnimateRipple");
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

            if (propertyName == "AnimateRipple")
            {
                SetRippleAnimation();
            }

            if (propertyName == "Status")
            {
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

        private static void OnAnimateRippleChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Card c = sender as Card;
            if (c != null)
            {
                c.OnPropertyChanged("AnimateRipple");
            }
        }

        private void SetRippleAnimation()
        {
            if (AnimateRipple)
            {
                StartRippleAnimation();
            }
            else
            {
                StopRippleAnimation();
            }
        }

        private void ConstructRippleAnimation()
        {
            try
            {
                var animationResourceKey = "ripple";
                var animationFileName = Application.Current.Resources[animationResourceKey].ToString();
                string animationJson;

                using (var sr = new StreamReader(Application.GetResourceStream(new Uri(animationFileName)).Stream))
                {
                    animationJson = sr.ReadToEnd();
                }

                rippleAnimation = new LottieAnimationView
                {
                    Name = "RippleAnimation",
                    DefaultCacheStrategy = LottieAnimationView.CacheStrategy.Strong,
                    Speed = 0.5,
                    AutoPlay = false,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                rippleAnimation.SetAnimationFromJsonAsync(animationJson, animationResourceKey);
            }
            catch (Exception)
            {
                ErrorHandling.ErrorHandler.WriteToLog("Failed to construct Lottie animation.", ErrorHandling.LogLevel.Error);
            }
        }

        private void StartRippleAnimation()
        {
            if (rippleAnimation == null)
            {
                return;
            }

            CardBorder.Child = rippleAnimation;
            rippleAnimation.Visibility = Visibility.Visible;
            rippleAnimation.PlayAnimation();
        }

        private void StopRippleAnimation()
        {
            if (rippleAnimation == null)
            {
                return;
            }

            CardBorder.Child = null;
            rippleAnimation.Visibility = Visibility.Hidden;
            rippleAnimation.PauseAnimation();
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
    }
}
