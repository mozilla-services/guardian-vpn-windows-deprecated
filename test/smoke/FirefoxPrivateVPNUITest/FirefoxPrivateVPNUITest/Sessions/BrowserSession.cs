// <copyright file="BrowserSession.cs" company="Mozilla">
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
    /// Firefox Browser session.
    /// </summary>
    public class BrowserSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string BrowserAppId = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserSession"/> class.
        /// </summary>
        public BrowserSession()
        {
            if (this.Session == null)
            {
                // Create a new session to bring up an instance of the FirefoxPrivateNetworkVPN application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", BrowserAppId);
                this.Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                this.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowUp + Keys.Command);
                Assert.IsNotNull(this.Session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                this.Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        /// <summary>
        /// Gets or sets Session.
        /// </summary>
        public WindowsDriver<WindowsElement> Session { get; set; }

        /// <summary>
        /// Dispose the browser session and close the browser.
        /// </summary>
        public void Dispose()
        {
            // Close the application and delete the session
            if (this.Session != null)
            {
                this.Session.Close();
                WindowsElement closeButton = this.Session.FindElementByName("Close Tabs");
                closeButton.Click();
                this.Session.Quit();
                this.Session = null;
            }
        }
    }
}
