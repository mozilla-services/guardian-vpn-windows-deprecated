// <copyright file="ButtonExtensions.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Dependency properties for all buttons.
    /// </summary>
    public class ButtonExtensions : DependencyObject
    {
        /// <summary>
        /// Dependency property for the background color of the button.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached("Background", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 5"]));

        /// <summary>
        /// Dependency property for the background color of the button on hover.
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.RegisterAttached("HoverBackground", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 10"]));

        /// <summary>
        /// Dependency property for the background color of the button when pressed.
        /// </summary>
        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.RegisterAttached("PressedBackground", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 20"]));

        /// <summary>
        /// Dependency property for the secondary background color of the button.
        /// </summary>
        public static readonly DependencyProperty SecondaryBackgroundProperty = DependencyProperty.RegisterAttached("SecondaryBackground", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 5"]));

        /// <summary>
        /// Dependency property for the secondary background color of the button on hover.
        /// </summary>
        public static readonly DependencyProperty SecondaryHoverBackgroundProperty = DependencyProperty.RegisterAttached("SecondaryHoverBackground", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 10"]));

        /// <summary>
        /// Dependency property for the secondary background color of the button when pressed.
        /// </summary>
        public static readonly DependencyProperty SecondaryPressedBackgroundProperty = DependencyProperty.RegisterAttached("SecondaryPressedBackground", typeof(SolidColorBrush), typeof(ButtonExtensions), new PropertyMetadata(Application.Current.Resources["Grey/Grey 20"]));

        /// <summary>
        /// Dependency property for the button's content.
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached("ButtonContent", typeof(object), typeof(ButtonExtensions), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property of the button icon's image source.
        /// </summary>
        public static readonly DependencyProperty IconUriProperty = DependencyProperty.RegisterAttached("IconUri", typeof(string), typeof(ButtonExtensions), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Dependency property of the button icon's image source on hover.
        /// </summary>
        public static readonly DependencyProperty HoverIconUriProperty = DependencyProperty.RegisterAttached("HoverIconUri", typeof(string), typeof(ButtonExtensions), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Dependency property for a value indicating whether a listview item associated with the button is marked for deletion.
        /// </summary>
        public static readonly DependencyProperty MarkForDeletionProperty = DependencyProperty.RegisterAttached("MarkForDeletion", typeof(bool), typeof(ButtonExtensions), new PropertyMetadata(false));

        /// <summary>
        /// Dependency property for a value indicating whether a listview item associated with the button is in the process of deletion.
        /// </summary>
        public static readonly DependencyProperty DeletingProperty = DependencyProperty.RegisterAttached("Deleting", typeof(bool), typeof(ButtonExtensions), new PropertyMetadata(false));

        /// <summary>
        /// Getter for <see cref="MarkForDeletionProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static bool GetMarkForDeletion(DependencyObject d)
        {
            return (bool)d.GetValue(MarkForDeletionProperty);
        }

        /// <summary>
        /// Setter for <see cref="MarkForDeletionProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetMarkForDeletion(DependencyObject d, bool value)
        {
            d.SetValue(MarkForDeletionProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="DeletingProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static bool GetDeleting(DependencyObject d)
        {
            return (bool)d.GetValue(DeletingProperty);
        }

        /// <summary>
        /// Setter for <see cref="DeletingProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetDeleting(DependencyObject d, bool value)
        {
            d.SetValue(DeletingProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="BackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(BackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="BackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="HoverBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetHoverBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(HoverBackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="HoverBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.<see cref="DependencyObject"/>.</param>
        public static void SetHoverBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(HoverBackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="PressedBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetPressedBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(PressedBackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="PressedBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetPressedBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(PressedBackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="SecondaryBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetSecondaryBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(SecondaryBackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="SecondaryBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetSecondaryBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(SecondaryBackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="SecondaryBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetSecondaryHoverBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(SecondaryHoverBackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="SecondaryBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetSecondaryHoverBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(SecondaryHoverBackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="SecondaryPressedBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static SolidColorBrush GetSecondaryPressedBackground(DependencyObject d)
        {
            return (SolidColorBrush)d.GetValue(SecondaryPressedBackgroundProperty);
        }

        /// <summary>
        /// Setter for <see cref="SecondaryPressedBackgroundProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetSecondaryPressedBackground(DependencyObject d, SolidColorBrush value)
        {
            d.SetValue(SecondaryPressedBackgroundProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="ButtonContentProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static object GetButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(ButtonContentProperty);
        }

        /// <summary>
        /// Setter for <see cref="ButtonContentProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetButtonContent(DependencyObject d, object value)
        {
            d.SetValue(ButtonContentProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="IconUriProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static string GetIconUri(DependencyObject d)
        {
            return (string)d.GetValue(IconUriProperty);
        }

        /// <summary>
        /// Setter for <see cref="IconUriProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetIconUri(DependencyObject d, string value)
        {
            d.SetValue(IconUriProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="HoverIconUriProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static string GetHoverIconUri(DependencyObject d)
        {
            return (string)d.GetValue(HoverIconUriProperty);
        }

        /// <summary>
        /// Setter for <see cref="HoverIconUriProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetHoverIconUri(DependencyObject d, string value)
        {
            d.SetValue(HoverIconUriProperty, value);
        }
    }
}
