// <copyright file="Avatar.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// The user's avatar associated with their FxA account.
    /// </summary>
    public class Avatar
    {
        private readonly TimeSpan maxAvatarDownloadTime = TimeSpan.FromSeconds(30);
        private Task<BitmapImage> downloadTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="Avatar"/> class.
        /// </summary>
        public Avatar()
        {
            InitializeCache();
        }

        /// <summary>
        /// Gets or sets the cache for avatar image.
        /// </summary>
        public ObjectCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the url of the avatar image.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Initializes cache.
        /// </summary>
        /// <param name="avatarUrl">The download URL of the avatar image.</param>
        /// <returns>The avatar download task, if applicable.</returns>
        public Task<BitmapImage> InitializeCache(string avatarUrl = null)
        {
            // Return the avatar download task if already running
            if (downloadTask != null && downloadTask.Status.Equals(TaskStatus.Running))
            {
                return downloadTask;
            }

            // Initialize the cache to the default avatar image
            Cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            Cache.Set("avatarImage", GetDefaultAvatarImage(), policy);

            if (avatarUrl == null)
            {
                return null;
            }

            // Attempt to retreive the avatar image if a download URL is provided
            downloadTask = Task.Run(() =>
            {
                var image = GetAvatarImageWithURL(avatarUrl);

                if (image != null)
                {
                    Url = avatarUrl;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Cache.Set("avatarImage", image, policy);
                    });

                    return image;
                }

                return null;
            });

            return downloadTask;
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        public void ClearCache()
        {
            Cache.Remove("avatarImage");
        }

        /// <summary>
        /// Gets the default avatar image.
        /// </summary>
        /// <returns>
        /// Default avatar image.
        /// </returns>
        public BitmapImage GetDefaultAvatarImage()
        {
            var defaultAvatarImage = new BitmapImage(new Uri("pack://application:,,,/UI/Resources/Icons/Generic/default-avatar.png"));
            defaultAvatarImage.Freeze();
            return defaultAvatarImage;
        }

        /// <summary>
        /// Gets the avatar image from Url.
        /// </summary>
        /// <returns>
        /// User's avatar image.
        /// </returns>
        /// <param name="url">The URL of the avatar image.</param>
        public BitmapImage GetAvatarImageWithURL(string url)
        {
            var image = new BitmapImage();

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException();
                }

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = (int)maxAvatarDownloadTime.TotalMilliseconds;

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        Bitmap bitmap = new Bitmap(stream);

                        using (var memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, ImageFormat.Png);
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = memoryStream;
                            image.EndInit();
                            image.Freeze();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
                image = null;
            }

            return image;
        }
    }
}
