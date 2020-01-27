// <copyright file="QuickAccessScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for Quick Access screen.
    /// </summary>
    internal class QuickAccessScreen
    {
        private AppiumWebElement titleElement;
        private AppiumWebElement subTitleElement;
        private AppiumWebElement descriptionElement;
        private AppiumWebElement continueButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public QuickAccessScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            this.titleElement = vpnSession.FindElementByName("Quick access");
            this.subTitleElement = vpnSession.FindElementByName("You can quickly access Firefox Private Network from your taskbar tray");
            this.descriptionElement = vpnSession.FindElementByName("Located next to the clock at the bottom right of your screen");
            this.continueButton = vpnSession.FindElementByName("Continue");
        }

        /// <summary>
        /// Get title on Quick Access screen.
        /// </summary>
        /// <returns>The tile string.</returns>
        public string GetTitle()
        {
            return this.titleElement.Text;
        }

        /// <summary>
        /// Get subtitle on Quick Access screen.
        /// </summary>
        /// <returns>The subtile string.</returns>
        public string GetSubTitle()
        {
            return this.subTitleElement.Text;
        }

        /// <summary>
        /// Get description on Quick Access screen.
        /// </summary>
        /// <returns>The description string.</returns>
        public string GetDescription()
        {
            return this.descriptionElement.Text;
        }

        /// <summary>
        /// Click Continue button.
        /// </summary>
        public void ClickContinueButton()
        {
            this.continueButton.Click();
        }
    }
}
