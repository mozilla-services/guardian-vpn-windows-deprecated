// <copyright file="BaseSession.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System.Drawing;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// Base session.
    /// </summary>
    public class BaseSession
    {
        /// <summary>
        /// Gets or sets Session.
        /// </summary>
        public WindowsDriver<WindowsElement> Session { get; set; }

        /// <summary>
        /// Set windows to a new postion.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public void SetWindowPosition(int x, int y)
        {
            this.Session.Manage().Window.Position = new Point(x, y);
        }

        /// <summary>
        /// Set window size.
        /// </summary>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        public void SetWindowSize(int newWidth, int newHeight)
        {
            this.Session.Manage().Window.Size = new Size(newWidth, newHeight);
        }
    }
}
