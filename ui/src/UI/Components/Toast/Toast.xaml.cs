// <copyright file="Toast.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FirefoxPrivateNetwork.UI.Components.Toast
{
    /// <summary>
    /// Visual style variations for the in-app toast.
    /// </summary>
    public enum Style
    {
        /// <summary>
        /// Used to convey information.
        /// </summary>
        Info,

        /// <summary>
        /// Used to convey error messages.
        /// </summary>
        Error,

        /// <summary>
        /// Used to convey success statuses.
        /// </summary>
        Success,
    }

    /// <summary>
    /// Display mode of the in-app toast.
    /// </summary>
    public enum Display
    {
        /// <summary>
        /// The toast will persist until explicitly dismissed by the user.
        /// </summary>
        Persistent,

        /// <summary>
        /// The toast will automatically be dismissed after the specificed display duration has elapsed.
        /// </summary>
        Ephemeral,
    }

    /// <summary>
    /// Display priority of the in-app toast.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Mandatory messages that will convey information impacting core functionality.
        /// </summary>
        Critical = 3,

        /// <summary>
        /// Crucial information requiring the user's attention.
        /// </summary>
        Important = 2,

        /// <summary>
        /// Generic message that is useful information to the user.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Message that may not necessarily require the user's immediate attention.
        /// </summary>
        Low = 0,
    }

    /// <summary>
    /// Interaction logic for Toast.xaml.
    /// </summary>
    public partial class Toast : UserControl
    {
        private const int ResizeWindowHeight = 40;
        private const Display DefaultDisplay = Display.Ephemeral;
        private const Priority DefaultPriority = Priority.Low;
        private const string DefaultEphemeralOwnerId = "Viewer";
        private const string DefaultPersistentOwnerId = "UpdateStackPanel";
        private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

        private static readonly DependencyProperty ClickEventHandlerProperty = DependencyProperty.Register("ClickEventHandler", typeof(EventHandler), typeof(Toast), new PropertyMetadata(null));
        private static readonly DependencyProperty DismissEventHandlerProperty = DependencyProperty.Register("DismissEventHandler", typeof(EventHandler), typeof(Toast), new PropertyMetadata(null));

        private Storyboard enterAnimation;
        private Storyboard exitAnimation;

        private string ownerId;
        private Type ownerType;
        private bool shown = false;
        private bool dismissed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Toast"/> class.
        /// </summary>
        /// <param name="style">Visual style variation of the toast.</param>
        /// <param name="message">Message to be displayed to the user.</param>
        /// <param name="ownerId">Id of the XAML control that the toast will be displayed within.</param>
        /// <param name="display">Display mode of the toast.</param>
        /// <param name="duration">Display duration for the toast.  Only relevant for ephemeral toasts.</param>
        /// <param name="priority">Display priority of the toast.</param>
        public Toast(Style style, ErrorHandling.UserFacingMessage message, string ownerId = "", Display display = DefaultDisplay, TimeSpan? duration = null, Priority priority = DefaultPriority)
        {
            this.Message = message;
            Content = message.TextBlock;
            Initialize(style, Content, ownerId, display, duration, priority);
        }

        /// <summary>
        /// Gets or sets the visual style variation of the toast.
        /// </summary>
        public new Style Style { get; set; }

        /// <summary>
        /// Gets or sets the message to be displayed to the user.
        /// </summary>
        public new TextBlock Content { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the XAML control that the toast will be displayed within.
        /// </summary>
        public Display Display { get; set; }

        /// <summary>
        /// Gets or sets the display duration for the toast.  Only relevant for ephemeral toasts.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the display priority of the toast.
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// Gets or sets the message to be displayed to the user.
        /// </summary>
        public ErrorHandling.UserFacingMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the event handler of the toast when clicked.
        /// </summary>
        public EventHandler ClickEventHandler
        {
            get
            {
                return (EventHandler)GetValue(ClickEventHandlerProperty);
            }

            set
            {
                SetValue(ClickEventHandlerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the event handler of the toast when dismissed.
        /// </summary>
        public EventHandler DismissEventHandler
        {
            get
            {
                return (EventHandler)GetValue(DismissEventHandlerProperty);
            }

            set
            {
                SetValue(DismissEventHandlerProperty, value);
            }
        }

        /// <summary>
        /// Adds the toast as a child of its owner control.
        /// </summary>
        public void AddToElement()
        {
            // Attempt to retrieve the owner control in the current main window.
            Panel owner = GetElementInCurrentWindow(ownerId);
            if (owner == null)
            {
                return;
            }

            // Only allow adding a toast to a StackPanel or Grid
            ownerType = owner.GetType();
            if (!(ownerType == typeof(StackPanel) || ownerType == typeof(Grid)))
            {
                return;
            }

            // Prevent displaying if the Toast is already being shown, or has been dismissed
            if (shown || dismissed)
            {
                return;
            }

            // Increase the height of the main window if the owner is a StackPanel
            if (ownerType == typeof(StackPanel))
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Window_ResizeHeight(ResizeWindowHeight);
            }

            // Add toast to its owner
            owner.Children.Add(this);
            shown = true;

            enterAnimation.Begin();

            if (Display == Display.Ephemeral)
            {
                var toastDurationTask = EphemeralToastDuration();
                toastDurationTask.ContinueWith(task =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Toast_Dismiss(null, null);
                    });
                });
            }
        }

        /// <summary>
        /// Removes the toast from its owner control.
        /// </summary>
        public void RemoveFromElement()
        {
            // Cannot remove the toast if it is not shown
            if (!shown)
            {
                return;
            }

            // Decrease the height of the main window if the owner is a StackPanel
            if (ownerType == typeof(StackPanel))
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Window_ResizeHeight(-ResizeWindowHeight);
            }

            // Remove toast from the owner
            var owner = GetElementInCurrentWindow(ownerId);
            if (owner != null)
            {
                exitAnimation.Completed += (sender, e) => { owner.Children.Remove(this); };
                exitAnimation.Begin();
            }

            shown = false;
        }

        /// <summary>
        /// Invokes the event handler that dismisses the toast.
        /// </summary>
        /// <param name="sender">Reference to the control that raised the event.</param>
        /// <param name="e">State information and event data.</param>
        public void Toast_Dismiss(object sender, RoutedEventArgs e)
        {
            if (DismissEventHandler != null)
            {
                DismissEventHandler(sender, e);
            }
            else
            {
                this.RemoveFromElement();
            }

            dismissed = true;
        }

        /// <summary>
        /// Invokes the event handler when the toast is clicked.
        /// </summary>
        /// <param name="sender">Reference to the control that raised the event.</param>
        /// <param name="e">State information and event data.</param>
        public void Toast_Click(object sender, RoutedEventArgs e)
        {
            ClickEventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// Refreshes the content within the toast by reconstructing the message textblock.
        /// </summary>
        public void RefreshContent()
        {
            Message.ConstructTextblock();
            Content = Message.TextBlock;
            ConfigureToast();
        }

        private void Initialize(Style style, TextBlock content, string ownerId, Display display, TimeSpan? duration, Priority priority)
        {
            this.ownerId = ownerId;
            this.Style = style;
            this.Content = content;
            this.Display = display;
            this.Priority = priority;

            if (duration == null)
            {
                this.Duration = DefaultDuration;
            }
            else
            {
                this.Duration = (TimeSpan)duration;
            }

            InitializeComponent();
            ConfigureToast();
        }

        private void ConfigureToast()
        {
            // Configure toast display mode specific attributes
            ConfigureToastCustomizations();

            // Configure toast generic attributes
            switch (Style)
            {
                case Style.Info:
                    Content.Foreground = (SolidColorBrush)Application.Current.FindResource("Grey/White");

                    ButtonExtensions.SetIconUri(this, Application.Current.FindResource("close-white").ToString());

                    ButtonExtensions.SetBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 50"));
                    ButtonExtensions.SetHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 70"));
                    ButtonExtensions.SetPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 80"));

                    ButtonExtensions.SetSecondaryBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 60"));
                    ButtonExtensions.SetSecondaryHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 70"));
                    ButtonExtensions.SetSecondaryPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Blue/Blue 80"));

                    break;

                case Style.Error:
                    Content.Foreground = (SolidColorBrush)Application.Current.FindResource("Grey/Grey 50");

                    ButtonExtensions.SetIconUri(this, Application.Current.FindResource("close").ToString());

                    ButtonExtensions.SetBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 40"));
                    ButtonExtensions.SetHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 60"));
                    ButtonExtensions.SetPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 70"));

                    ButtonExtensions.SetSecondaryBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 50"));
                    ButtonExtensions.SetSecondaryHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 60"));
                    ButtonExtensions.SetSecondaryPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Red/Red 70"));

                    break;

                case Style.Success:
                    Content.Foreground = (SolidColorBrush)Application.Current.FindResource("Grey/Grey 50");

                    ButtonExtensions.SetIconUri(this, Application.Current.FindResource("close").ToString());

                    ButtonExtensions.SetBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 40"));
                    ButtonExtensions.SetHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 60"));
                    ButtonExtensions.SetPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 70"));

                    ButtonExtensions.SetSecondaryBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 50"));
                    ButtonExtensions.SetSecondaryHoverBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 60"));
                    ButtonExtensions.SetSecondaryPressedBackground(this, (SolidColorBrush)Application.Current.FindResource("Green/Green 70"));

                    break;

                default:
                    break;
            }

            // Configure toast main content
            Content.TextWrapping = TextWrapping.Wrap;
            Content.TextAlignment = TextAlignment.Center;
            ButtonExtensions.SetButtonContent(this, Content);
        }

        private void ConfigureToastCustomizations()
        {
            switch (Display)
            {
                case Display.Ephemeral:
                    if (string.IsNullOrEmpty(ownerId))
                    {
                        ownerId = DefaultEphemeralOwnerId;
                    }

                    this.VerticalAlignment = VerticalAlignment.Bottom;
                    this.Margin = new Thickness(8, 0, 8, 8);
                    this.enterAnimation = this.FindResource("SlideUpAndFadeIn") as Storyboard;
                    this.exitAnimation = this.FindResource("FadeOut") as Storyboard;
                    break;

                case Display.Persistent:
                    if (string.IsNullOrEmpty(ownerId))
                    {
                        ownerId = DefaultPersistentOwnerId;
                    }

                    this.VerticalAlignment = VerticalAlignment.Top;
                    this.Margin = new Thickness(8, 8, 8, 0);
                    this.enterAnimation = this.FindResource("SlideDownAndFadeIn") as Storyboard;
                    this.exitAnimation = this.FindResource("FadeOut") as Storyboard;
                    break;

                default:
                    break;
            }
        }

        private Panel GetElementInCurrentWindow(string elementName)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return null;
            }

            var element = (Panel)mainWindow.FindName(elementName);
            return element;
        }

        private async Task EphemeralToastDuration()
        {
            await Task.Delay(Duration);
        }
    }
}
