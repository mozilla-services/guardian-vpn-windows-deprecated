// <copyright file="LastOnboardingScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for main screen of VPN client.
    /// </summary>
    internal class LastOnboardingScreen
    {
        private WindowsElement closeButton;
        private WindowsElement image;
        private AppiumWebElement title;
        private AppiumWebElement subtitle;
        private WindowsElement getStartedButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastOnboardingScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        /// <param name="viewClassName">onboarding screen view class name.</param>
        public LastOnboardingScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var onboardingView = vpnSession.FindElementByClassName("OnboardingView4");
            var textBlocks = onboardingView.FindElementsByClassName("TextBlock");
            Assert.IsTrue(textBlocks.Count > 2);
            this.closeButton = vpnSession.FindElementByName("Close");
            this.image = vpnSession.FindElementByClassName("Image");
            this.title = textBlocks[0];
            this.subtitle = textBlocks[1];
            this.getStartedButton = vpnSession.FindElementByName("Get started");
            Assert.IsNotNull(this.closeButton);
            Assert.IsNotNull(this.image);
            Assert.IsNotNull(this.title);
            Assert.IsNotNull(this.subtitle);
            Assert.IsNotNull(this.getStartedButton);
        }

        /// <summary>
        /// Get title on onboarding screen.
        /// </summary>
        /// <returns>The tile string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get subtitle on onboarding screen.
        /// </summary>
        /// <returns>The tile string.</returns>
        public string GetSubTitle()
        {
            return this.subtitle.Text;
        }

        /// <summary>
        /// Get the text on Next button.
        /// </summary>
        /// <returns>The text on Next button.</returns>
        public string GetGetStartedButtonText()
        {
            return this.getStartedButton.Text;
        }

        /// <summary>
        /// Click Close.
        /// </summary>
        public void ClickClose()
        {
            this.closeButton.Click();
        }

        /// <summary>
        /// Click Get started button.
        /// </summary>
        public void ClickGetStartedButton()
        {
            this.getStartedButton.Click();
        }
    }
}
