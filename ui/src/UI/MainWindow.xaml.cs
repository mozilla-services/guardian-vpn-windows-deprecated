// <copyright file="MainWindow.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double WindowDecorationMinWidth = 1060;
        private const double WindowShadowMinWidth = 520;
        private static readonly TimeSpan WindowResizeHeightDuration = TimeSpan.FromSeconds(0.2);

        private bool isDisposed = false;
        private double windowCurrentHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Frame slide direction during transition when navigating to a different frame source.
        /// </summary>
        public enum SlideDirection
        {
            /// <summary>
            /// View frame slides to the left on transition.
            /// </summary>
            Left,

            /// <summary>
            /// View frame slides to the right on transition.
            /// </summary>
            Right,

            /// <summary>
            /// View frame slides up on transition.
            /// </summary>
            Up,

            /// <summary>
            /// View frame slides down on transition.
            /// </summary>
            Down,

            /// <summary>
            /// View frame does not slide on transition.
            /// </summary>
            None,
        }

        /// <summary>
        /// Behaviour for clean up upon closing the MainWindow.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Resizes the window height.
        /// </summary>
        /// <param name="resizeHeight">Height to increase/decrease the window by.</param>
        public void Window_ResizeHeight(double resizeHeight)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation expand = new DoubleAnimation { To = windowCurrentHeight + resizeHeight, Duration = WindowResizeHeightDuration };
            Storyboard.SetTarget(expand, this);
            Storyboard.SetTargetProperty(expand, new PropertyPath("Height"));
            sb.Children.Add(expand);

            windowCurrentHeight += resizeHeight;
            MinHeight = windowCurrentHeight;
            this.BeginStoryboard(sb);
        }

        /// <summary>
        /// Refreshes the VPN server list from cache.
        /// </summary>
        public void RefreshServers()
        {
            Manager.MainWindowViewModel.CountryServerList = FxA.Cache.FxAServerList.GetServerListCountryItems();
            Manager.MainWindowViewModel.RefreshServerListSelectedItem();
        }

        /// <summary>
        /// Navigates the current view frame source to a different view control.
        /// </summary>
        /// <param name="newView">View control to navigate to.</param>
        /// <param name="slideDirection">Frame navigation transition slide direction.</param>
        public void NavigateToView(UserControl newView, SlideDirection slideDirection)
        {
            Storyboard sb;
            string resource = string.Empty;

            switch (slideDirection)
            {
                case SlideDirection.Left:
                    resource = "slideScreenOutToLeftAndFade";
                    break;
                case SlideDirection.Right:
                    resource = "slideScreenOutToRightAndFade";
                    break;
                case SlideDirection.Up:
                    resource = "slideScreenOutToAboveAndFade";
                    break;
                case SlideDirection.Down:
                    resource = "slideScreenOutToBelowAndFade";
                    break;
            }

            if (!string.IsNullOrEmpty(resource))
            {

                sb = (this.FindResource(resource) as Storyboard).Clone();
                Storyboard.SetTarget(sb, Viewer);
                sb.Completed += (sender, e) => { SlideCompleted(newView, slideDirection); };
                sb.Begin();

                return;
            }

            SlideCompleted(newView, SlideDirection.None);
        }

        /// <summary>
        /// Helper function that releases RAM when MainWindow is minimized.
        /// </summary>
        /// <param name="disposing">Whether the current class is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                // Releases RAM and signals garbage collector to clean up unused resources.
                FirefoxPrivateNetwork.App.Current.MainWindow = null;
                GC.Collect();
                Windows.Kernel32.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

                isDisposed = true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowLeftDecoration.Visibility = this.ActualWidth >= WindowDecorationMinWidth ? Visibility.Visible : Visibility.Hidden;
            WindowRightDecoration.Visibility = this.ActualWidth >= WindowDecorationMinWidth ? Visibility.Visible : Visibility.Hidden;
            WindowLeftShadow.Visibility = this.ActualWidth >= WindowShadowMinWidth ? Visibility.Visible : Visibility.Hidden;
            WindowRightShadow.Visibility = this.ActualWidth >= WindowShadowMinWidth ? Visibility.Visible : Visibility.Hidden;
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            // Configure properties for the toast manager
            Manager.ToastManager.MainWindowActive = false;
        }

        private void ViewFrame_ContentRendered(object sender, EventArgs e)
        {
            if (new List<Type> { typeof(ConnectionView), typeof(MainView) }.Contains(ViewFrame.Content.GetType()))
            {
                RefreshServers();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // Initialize the main window's data context
            DataContext = Manager.MainWindowViewModel;

            // Configure properties for the toast manager
            Manager.ToastManager.MainWindowActive = true;
            windowCurrentHeight = this.Height;

            // Specify the main window's view frame source
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            if (Manager.MainWindowViewModel.ViewFrameSource == null)
            {
                mainWindow.NavigateToView((UserControl)Activator.CreateInstance(Manager.MainWindowViewModel.InitialViewFrameSourceType), MainWindow.SlideDirection.None);
            }
            else
            {
                mainWindow.NavigateToView(Manager.MainWindowViewModel.ViewFrameSource, MainWindow.SlideDirection.None);
            }
        }

        private void SlideCompleted(UserControl newView, SlideDirection slideDirection)
        {
            ViewFrame.Navigate(newView);
            Manager.MainWindowViewModel.ViewFrameSource = newView;

            if (slideDirection == SlideDirection.None)
            {
                return;
            }

            Storyboard sb = new Storyboard();

            switch (slideDirection)
            {
                case SlideDirection.Left:
                    sb = (this.FindResource("slideScreenInFromRightAndShow") as Storyboard).Clone();
                    break;
                case SlideDirection.Right:
                    sb = (this.FindResource("slideScreenInFromLeftAndShow") as Storyboard).Clone();
                    break;
                case SlideDirection.Up:
                    sb = (this.FindResource("slideScreenInFromBelowAndShow") as Storyboard).Clone();
                    break;
                case SlideDirection.Down:
                    sb = (this.FindResource("slideScreenInFromAboveAndShow") as Storyboard).Clone();
                    break;
                default:
                    break;
            }

            Storyboard.SetTarget(sb, Viewer);
            sb.Begin();
        }
    }
}
