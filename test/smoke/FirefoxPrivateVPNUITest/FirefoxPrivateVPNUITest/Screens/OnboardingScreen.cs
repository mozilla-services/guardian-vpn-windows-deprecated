// <copyright file="OnboardingScreen.cs" company="Mozilla">
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
    internal class OnboardingScreen
    {
        private WindowsElement closeButton;
        private AppiumWebElement skip;
        private WindowsElement image;
        private AppiumWebElement title;
        private AppiumWebElement subtitle;
        private WindowsElement nextButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        /// <param name="viewClassName">onboarding screen view class name.</param>
        public OnboardingScreen(WindowsDriver<WindowsElement> vpnSession, string viewClassName)
        {
            var onboardingView = vpnSession.FindElementByClassName(viewClassName);
            var textBlocks = onboardingView.FindElementsByClassName("TextBlock");
            Assert.IsTrue(textBlocks.Count > 3);
            this.closeButton = vpnSession.FindElementByName("Close");
            this.skip = textBlocks[0];
            this.image = vpnSession.FindElementByClassName("Image");
            this.title = textBlocks[1];
            this.subtitle = textBlocks[2];
            this.nextButton = vpnSession.FindElementByName("Next");
            Assert.IsNotNull(this.closeButton);
            Assert.IsNotNull(this.skip);
            Assert.IsNotNull(this.image);
            Assert.IsNotNull(this.title);
            Assert.IsNotNull(this.subtitle);
            Assert.IsNotNull(this.nextButton);
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
        /// Get the text on Skip.
        /// </summary>
        /// <returns>The text on Skip.</returns>
        public string GetSkipText()
        {
            return this.skip.Text;
        }

        /// <summary>
        /// Get the text on Next button.
        /// </summary>
        /// <returns>The text on Next button.</returns>
        public string GetNextButtonText()
        {
            return this.nextButton.Text;
        }

        /// <summary>
        /// Click Skip.
        /// </summary>
        public void ClickSkip()
        {
            this.skip.Click();
        }

        /// <summary>
        /// Click Next button.
        /// </summary>
        public void ClickNextButton()
        {
            this.nextButton.Click();
        }

        /// <summary>
        /// Click Close.
        /// </summary>
        public void ClickClose()
        {
            this.closeButton.Click();
        }
    }
}
