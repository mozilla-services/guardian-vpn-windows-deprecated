// <copyright file="SettingScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using System.Threading;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Setting screen.
    /// </summary>
    internal class SettingScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement titleText;
        private AppiumWebElement signOutButton;
        private AppiumWebElement scrollDownButton;
        private WindowsDriver<WindowsElement> vpnSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public SettingScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            if (this.vpnSession == null)
            {
                this.vpnSession = vpnSession;
            }

            this.backButton = vpnSession.FindElementByName("Back");
            this.titleText = vpnSession.FindElementByName("Settings");
            this.signOutButton = vpnSession.FindElementByName("Sign out");
            this.scrollDownButton = vpnSession.FindElementByAccessibilityId("PART_LineDownButton");
        }

        /// <summary>
        /// Get title on Setting screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.titleText.Text;
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }

        /// <summary>
        /// Click the Sign Out button.
        /// </summary>
        public void ClickSignOutButton()
        {
            this.scrollDownButton.Click();
            this.vpnSession.Mouse.MouseDown(null);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            this.vpnSession.Mouse.MouseUp(null);
            this.signOutButton.Click();
        }
    }
}
