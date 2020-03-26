// <copyright file="Avatar.xaml.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Interaction logic for Avatar.xaml.
    /// </summary>
    public partial class Avatar : UserControl
    {
        /// <summary>
        /// Dependency property for the profile picture image source.
        /// </summary>
        public static readonly DependencyProperty AvatarImageProperty = DependencyProperty.Register("Url", typeof(ImageSource), typeof(Avatar));

        /// <summary>
        /// Dependency property for the avatar size.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(Avatar));

        private bool initialLoad = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Avatar"/> class.
        /// </summary>
        public Avatar()
        {
            InitializeComponent();
            ConfigureProfileImage();
        }

        /// <summary>
        /// Gets or sets the profile picture image source.
        /// </summary>
        public ImageSource AvatarImage
        {
            get
            {
                return (ImageSource)GetValue(AvatarImageProperty);
            }

            set
            {
                SetValue(AvatarImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the avatar size.
        /// </summary>
        public double Size
        {
            get
            {
                return (double)GetValue(SizeProperty);
            }

            set
            {
                SetValue(SizeProperty, value);
            }
        }

        /// <summary>
        /// Configures the avatar image based on the cache.
        /// </summary>
        public void ConfigureProfileImage()
        {
            // Initialize profile image with the avatar cache
            var image = Manager.Account.Avatar.Cache.Get("avatarImage");
            AvatarImage = (BitmapImage)image;

            // Fetch the latest account info, and re-download the avatar image if out of sync with the cache
            Manager.AccountInfoUpdater.ForcePollAccountInfo().ContinueWith(_ =>
            {
                if (Manager.Account.Config.FxALogin.User.Avatar != null && Manager.Account.Config.FxALogin.User.Avatar != Manager.Account.Avatar.Url)
                {
                    var avatarDownloadTask = Manager.Account.Avatar.InitializeCache(avatarUrl: Manager.Account.Config.FxALogin.User.Avatar);

                    if (avatarDownloadTask != null)
                    {
                        avatarDownloadTask.ContinueWith(task =>
                        {
                            if (task.Result != null)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    AvatarImage = task.Result;
                                    ImageBrush profileImage = (ImageBrush)ProfileImageButton.Template.FindName("ProfileImage", ProfileImageButton);
                                    profileImage.ImageSource = AvatarImage;
                                });
                            }
                        });
                    }
                }
            });
        }

        private void Avatar_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialLoad)
            {
                initialLoad = true;
            }
            else
            {
                ConfigureProfileImage();
            }
        }
    }
}
