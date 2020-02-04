// <copyright file="VerifyAccountScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for verify account screen of VPN client.
    /// </summary>
    internal class VerifyAccountScreen
    {
        private AppiumWebElement titleElement;
        private AppiumWebElement spinner;
        private AppiumWebElement cancelTryAgainButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyAccountScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public VerifyAccountScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            WindowsElement landingView = vpnSession.FindElementByClassName("VerifyAccountView");
            var titles = landingView.FindElementsByClassName("TextBlock");
            this.titleElement = titles[0];
            this.cancelTryAgainButton = titles[1];
            this.spinner = landingView.FindElementByClassName("Spinner");
            Assert.IsNotNull(this.spinner);
        }

        /// <summary>
        /// Get title on VerifyAccount screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.titleElement.Text;
        }

        /// <summary>
        /// Get the text on Cancel and try again button.
        /// </summary>
        /// <returns>The text on Cancel and try again Button.</returns>
        public string GetCancelTryAgainButtonText()
        {
            return this.cancelTryAgainButton.Text;
        }

        /// <summary>
        /// Click the Cancel and try again button.
        /// </summary>
        public void ClickCancelTryAgainButton()
        {
            this.cancelTryAgainButton.Click();
        }
    }
}
