// <copyright file="Ripple.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for Ripple.xaml.
    /// </summary>
    public partial class Ripple : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Dependency property indicating whether the animation should be running or not.
        /// </summary>
        public static readonly DependencyProperty RunningProperty = DependencyProperty.Register("Running", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

        private const int FrameWidth = 328;
        private const int FrameHeight = 318;
        private const int FrameTimeoutMs = 35;
        private const int FrameCount = 45;

        private Task animationTask;
        private CancellationTokenSource animationTaskCts;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ripple"/> class.
        /// </summary>
        public Ripple()
        {
            InitializeComponent();
            SetAnimation();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the component animation is running or not.
        /// </summary>
        public bool Running
        {
            get
            {
                return (bool)GetValue(RunningProperty);
            }

            set
            {
                SetValue(RunningProperty, value);
                OnPropertyChanged("Running");
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

            if (propertyName == "Running")
            {
                SetAnimation();
            }
        }

        private void SetAnimation()
        {
            if (Running == true)
            {
                if (animationTask != null && !animationTask.IsCompleted)
                {
                    // Animation is already running, no need to proceed
                    return;
                }

                // Fade in the animation
                Dispatcher.Invoke(() =>
                {
                    RippleImage.Opacity = 0;
                    if (this.FindResource("FadeInAnimation") is Storyboard fadeInStoryboard)
                    {
                        BeginStoryboard(fadeInStoryboard);
                    }
                });

                animationTaskCts = new CancellationTokenSource();

                // Invoke the animation scroller
                animationTask = new Task(() =>
                    {
                        var position = 0;
                        while (!animationTaskCts.IsCancellationRequested)
                        {
                            var newPos = new Rect(position, 0, FrameWidth, FrameHeight);
                            Dispatcher.Invoke(() =>
                            {
                                RipplePosition.Rect = newPos;
                            });

                            position += FrameWidth;
                            animationTaskCts.Token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(FrameTimeoutMs));

                            if (Math.Abs(position) >= FrameWidth * FrameCount)
                            {
                                position = 0;
                            }
                        }
                    },
                    animationTaskCts.Token,
                    TaskCreationOptions.LongRunning
                );

                animationTask.Start();
            }
            else
            {
                if (animationTask == null || animationTask.IsCompleted)
                {
                    // Animation thread is not active, no need to proceed
                    return;
                }

                animationTaskCts.Cancel();

                // Fade out and reset to first position
                var newPos = new Rect(0, 0, FrameWidth, FrameHeight);
                Dispatcher.Invoke(() =>
                {
                    if (this.FindResource("FadeOutAnimation") is Storyboard fadeOutStoryboard)
                    {
                        BeginStoryboard(fadeOutStoryboard);
                    }

                    RipplePosition.Rect = newPos;
                });
            }
        }
    }
}
