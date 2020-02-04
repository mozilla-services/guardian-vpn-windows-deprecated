// <copyright file="DesktopSession.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Remote;

    /// <summary>
    /// Firefox Browser session.
    /// </summary>
    public class DesktopSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopSession"/> class.
        /// </summary>
        public DesktopSession()
        {
            if (this.Session == null)
            {
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", "Root");
                this.Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                this.Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        /// <summary>
        /// Gets or sets Session.
        /// </summary>
        public WindowsDriver<WindowsElement> Session { get; set; }

        /// <summary>
        /// Dispose the desktop session.
        /// </summary>
        public void Dispose()
        {
            // Close the application and delete the session
            if (this.Session != null)
            {
                this.Session.Close();
                this.Session.Quit();
                this.Session = null;
            }
        }

        /// <summary>
        /// Close Firefox VPN client.
        /// </summary>
        public void CloseVPNClient()
        {
            var notification = this.Session.FindElementByName("Notification Chevron");
            notification.Click();
            var clientTray = this.Session.FindElementByName("Firefox Private Network VPN - Disconnected");
            this.Session.Mouse.ContextClick(clientTray.Coordinates);
            var exitItem = this.Session.FindElementByName("E_xit");
            exitItem.Click();
            notification.Click();
        }
    }
}
