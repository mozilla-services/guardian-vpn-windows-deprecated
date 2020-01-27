// <copyright file="FirefoxPrivateVPNSession.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Remote;

    /// <summary>
    /// Firefox Private VPN session.
    /// </summary>
    public class FirefoxPrivateVPNSession : IDisposable
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string FirefoxPrivateVPNAppId = @"C:\Program Files\Mozilla\Firefox Private Network VPN\FirefoxPrivateNetworkVPN.exe";

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxPrivateVPNSession"/> class.
        /// </summary>
        public FirefoxPrivateVPNSession()
        {
            if (this.Session == null)
            {
                // Create a new session to bring up an instance of the FirefoxPrivateNetworkVPN application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", FirefoxPrivateVPNAppId);
                appCapabilities.SetCapability("deviceName", "WindowsPC");
                this.Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(this.Session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                this.Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        /// <summary>
        /// Gets session.
        /// </summary>
        public WindowsDriver<WindowsElement> Session { get; private set; }

        /// <summary>
        /// Dispose the VPN session and close the app.
        /// </summary>
        public void Dispose()
        {
            // Close the application and delete the session
            if (this.Session != null)
            {
                this.Session.Quit();
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", "Root");
                var desktopSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
                desktopSession.FindElementByName("Notification Chevron").Click();
                var clientTray = desktopSession.FindElementByName("Firefox Private Network VPN - Disconnected");
                desktopSession.Mouse.ContextClick(clientTray.Coordinates);
                var exitItem = desktopSession.FindElementByName("_Exit");
                exitItem.Click();
                desktopSession.Quit();
                this.Session = null;
            }
        }

        /// <summary>
        /// Minimize the VPN window.
        /// </summary>
        public void MinimizeWindows()
        {
            this.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowDown + Keys.Command);
        }

        /// <summary>
        /// Maximize the VPN window.
        /// </summary>
        public void MaxmizeWindows()
        {
            this.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowUp + Keys.ArrowUp + Keys.Command);
        }
    }
}
