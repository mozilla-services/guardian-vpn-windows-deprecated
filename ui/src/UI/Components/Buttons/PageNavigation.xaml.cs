// <copyright file="PageNavigation.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for PageNavigation.xaml.
    /// </summary>
    public partial class PageNavigation : Button
    {
        /// <summary>
        /// Dependency property for the title text of the page navigation button.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PageNavigation), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Dependency property for the title icon image source.
        /// </summary>
        public static readonly DependencyProperty TitleIconUrlProperty = DependencyProperty.Register("TitleIconUrl", typeof(string), typeof(PageNavigation));

        /// <summary>
        /// Dependency property for the title text color.
        /// </summary>
        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register("TitleColor", typeof(SolidColorBrush), typeof(PageNavigation), new PropertyMetadata(Application.Current.Resources["Grey/Grey 50"]));

        /// <summary>
        /// Dependency property for the subtitle text of the page navigation button.
        /// </summary>
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register("Subtitle", typeof(string), typeof(PageNavigation), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Dependency property for the subtitle text color.
        /// </summary>
        public static readonly DependencyProperty SubtitleColorProperty = DependencyProperty.Register("SubtitleColor", typeof(SolidColorBrush), typeof(PageNavigation), new PropertyMetadata(Application.Current.Resources["Grey/Grey 50"]));

        /// <summary>
        /// Dependency property for the subtitle icon image source.
        /// </summary>
        public static readonly DependencyProperty SubtitleIconUrlProperty = DependencyProperty.Register("SubtitleIconUrl", typeof(string), typeof(PageNavigation));

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNavigation"/> class.
        /// </summary>
        public PageNavigation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the title text of the page navigation button.
        /// </summary>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title text color.
        /// </summary>
        public SolidColorBrush TitleColor
        {
            get
            {
                return (SolidColorBrush)GetValue(TitleColorProperty);
            }

            set
            {
                SetValue(TitleColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title icon image source.
        /// </summary>
        public string TitleIconUrl
        {
            get
            {
                return (string)GetValue(TitleIconUrlProperty);
            }

            set
            {
                SetValue(TitleIconUrlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the subtitle text of the page navigation button.
        /// </summary>
        public string Subtitle
        {
            get
            {
                return (string)GetValue(SubtitleProperty);
            }

            set
            {
                SetValue(SubtitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the subtitle text color.
        /// </summary>
        public SolidColorBrush SubtitleColor
        {
            get
            {
                return (SolidColorBrush)GetValue(SubtitleColorProperty);
            }

            set
            {
                SetValue(SubtitleColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the subtitle icon image source.
        /// </summary>
        public string SubtitleIconUrl
        {
            get
            {
                return (string)GetValue(SubtitleIconUrlProperty);
            }

            set
            {
                SetValue(SubtitleIconUrlProperty, value);
            }
        }
    }
}
