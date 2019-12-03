// <copyright file="PopupMessage.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for PopupMessage.xaml.
    /// </summary>
    public partial class PopupMessage : UserControl
    {
        /// <summary>
        /// Title of the popup control.
        /// </summary>
        public static readonly DependencyProperty PopupTitleProperty = DependencyProperty.Register("PopupTitle", typeof(string), typeof(PopupMessage));

        /// <summary>
        /// Content of the poup control.
        /// </summary>
        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(string), typeof(PopupMessage));

        /// <summary>
        /// Associated user device for a device deletion confirmation popup.
        /// </summary>
        public static readonly DependencyProperty DeviceToDeleteProperty = DependencyProperty.Register("DeviceToDelete", typeof(Models.DeviceListItem), typeof(PopupMessage));

        /// <summary>
        /// Flag that indicates if the cancel button is displayed in the popup or not.
        /// </summary>
        public static readonly DependencyProperty CancelButtonProperty = DependencyProperty.Register("CancelButton", typeof(bool), typeof(PopupMessage), new PropertyMetadata(false));

        /// <summary>
        /// Event handler for the cancel button.
        /// </summary>
        public static readonly DependencyProperty CancelButtonEventHandlerProperty = DependencyProperty.Register("CancelButtonEventHandler", typeof(EventHandler), typeof(PopupMessage));

        /// <summary>
        /// Flag that indicates if the remove button is displayed in the popup or not.
        /// </summary>
        public static readonly DependencyProperty RemoveButtonProperty = DependencyProperty.Register("RemoveButton", typeof(bool), typeof(PopupMessage), new PropertyMetadata(false));

        /// <summary>
        /// Event handler for the remove button.
        /// </summary>
        public static readonly DependencyProperty RemoveButtonEventHandlerProperty = DependencyProperty.Register("RemoveButtonEventHandler", typeof(EventHandler), typeof(PopupMessage));

        /// <summary>
        /// Flag that indicates if the OK button is displayed in the popup or not.
        /// </summary>
        public static readonly DependencyProperty OkButtonProperty = DependencyProperty.Register("OkButton", typeof(bool), typeof(PopupMessage), new PropertyMetadata(false));

        /// <summary>
        /// Event handler for the OK button.
        /// </summary>
        public static readonly DependencyProperty OkButtonEventHandlerProperty = DependencyProperty.Register("OkButtonEventHandler", typeof(EventHandler), typeof(PopupMessage));

        /// <summary>
        /// Event handler that manages behaviour when the popup is closed.
        /// </summary>
        public static readonly DependencyProperty PopupClosdEventHandlerProperty = DependencyProperty.Register("PopupClosedEventHandler", typeof(EventHandler), typeof(PopupMessage));

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupMessage"/> class.
        /// </summary>
        public PopupMessage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the title of the popup.
        /// </summary>
        public string PopupTitle
        {
            get
            {
                return (string)GetValue(PopupTitleProperty);
            }

            set
            {
                SetValue(PopupTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the popup.
        /// </summary>
        public string PopupContent
        {
            get
            {
                return (string)GetValue(PopupContentProperty);
            }

            set
            {
                SetValue(PopupContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the associated user device for a device deletion confirmation popup.
        /// </summary>
        public Models.DeviceListItem DeviceToDelete
        {
            get
            {
                return (Models.DeviceListItem)GetValue(DeviceToDeleteProperty);
            }

            set
            {
                SetValue(DeviceToDeleteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel button is displayed in the popup or not.
        /// </summary>
        public bool CancelButton
        {
            get
            {
                return (bool)GetValue(CancelButtonProperty);
            }

            set
            {
                SetValue(CancelButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the event handler for the cancel button.
        /// </summary>
        public EventHandler CancelButtonEventHandler
        {
            get
            {
                return (EventHandler)GetValue(CancelButtonEventHandlerProperty);
            }

            set
            {
                SetValue(CancelButtonEventHandlerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the remove button is displayed in the popup or not.
        /// </summary>
        public bool RemoveButton
        {
            get
            {
                return (bool)GetValue(RemoveButtonProperty);
            }

            set
            {
                SetValue(RemoveButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the event handler for the remove button.
        /// </summary>
        public EventHandler RemoveButtonEventHandler
        {
            get
            {
                return (EventHandler)GetValue(RemoveButtonEventHandlerProperty);
            }

            set
            {
                SetValue(RemoveButtonEventHandlerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the OK button is displayed in the popup or not.
        /// </summary>
        public bool OkButton
        {
            get
            {
                return (bool)GetValue(OkButtonProperty);
            }

            set
            {
                SetValue(OkButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the event handler for the OK button.
        /// </summary>
        public EventHandler OkButtonEventHandler
        {
            get
            {
                return (EventHandler)GetValue(OkButtonEventHandlerProperty);
            }

            set
            {
                SetValue(OkButtonEventHandlerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the event handler that manages behaviour when the popup is closed.
        /// </summary>
        public EventHandler PopupClosedEventHandler
        {
            get
            {
                return (EventHandler)GetValue(PopupClosdEventHandlerProperty);
            }

            set
            {
                SetValue(PopupClosdEventHandlerProperty, value);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelButtonEventHandler(sender, e);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveButtonEventHandler(sender, e);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OkButtonEventHandler(sender, e);
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            PopupClosedEventHandler(sender, e);
        }
    }
}
