// <copyright file="MainScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for main screen of VPN client.
    /// </summary>
    internal class MainScreen
    {
        private AppiumWebElement titleElement;
        private AppiumWebElement settingButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public MainScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            this.titleElement = vpnSession.FindElementByName("VPN is off");
            this.settingButton = vpnSession.FindElementByName("Settings");
        }

        /// <summary>
        /// Get title on main screen.
        /// </summary>
        /// <returns>The tile string.</returns>
        public string GetTitle()
        {
            return this.titleElement.Text;
        }

        /// <summary>
        /// Click Settings button.
        /// </summary>
        public void ClickSettingsButton()
        {
            this.settingButton.Click();
        }
    }
}
