// <copyright file="LandingScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for landing screen of VPN client.
    /// </summary>
    internal class LandingScreen
    {
        private AppiumWebElement titleElement;
        private AppiumWebElement subTitleElement;
        private AppiumWebElement getStartedButton;
        private AppiumWebElement learnMoreHyperlink;

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public LandingScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            WindowsElement landingView = vpnSession.FindElementByClassName("LandingView");
            var titles = landingView.FindElementsByClassName("TextBlock");
            this.titleElement = titles[0];
            this.subTitleElement = titles[1];
            this.getStartedButton = landingView.FindElementByName("Get started");
            this.learnMoreHyperlink = landingView.FindElementByName("Learn more");
        }

        /// <summary>
        /// Get title on landing screen.
        /// </summary>
        /// <returns>The tile string.</returns>
        public string GetTitle()
        {
            return this.titleElement.Text;
        }

        /// <summary>
        /// Get subtitle on landing screen.
        /// </summary>
        /// <returns>The subtitle string.</returns>
        public string GetSubTitle()
        {
            return this.subTitleElement.Text;
        }

        /// <summary>
        /// Get the text on Get Started button.
        /// </summary>
        /// <returns>The text on Get Started Button.</returns>
        public string GetStartedButtonText()
        {
            return this.getStartedButton.Text;
        }

        /// <summary>
        /// Click the Get Started button.
        /// </summary>
        public void ClickGetStartedButton()
        {
            this.getStartedButton.Click();
        }

        /// <summary>
        /// Get the text on Learn More hyperlink.
        /// </summary>
        /// <returns>The text on Learn More hyperlink.</returns>
        public string GetLearnMoreHyperlinkText()
        {
            return this.learnMoreHyperlink.Text;
        }

        /// <summary>
        /// Click Learn More Hyperlink.
        /// </summary>
        public void ClickLearnMoreHyperlink()
        {
            this.learnMoreHyperlink.Click();
        }
    }
}
