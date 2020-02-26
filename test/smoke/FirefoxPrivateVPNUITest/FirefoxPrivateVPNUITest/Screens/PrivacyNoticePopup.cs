// <copyright file="PrivacyNoticePopup.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Privacy Notice popup.
    /// </summary>
    internal class PrivacyNoticePopup
    {
        private AppiumWebElement yesButton;
        private AppiumWebElement noButton;
        private AppiumWebElement privacyNoticeDetails;
        private AppiumWebElement imageIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivacyNoticePopup"/> class.
        /// </summary>
        /// <param name="desktopSession">VPN session.</param>
        public PrivacyNoticePopup(WindowsDriver<WindowsElement> desktopSession)
        {
            var privacyNoticePopup = desktopSession.FindElementByName("Privacy notice");
            var staticElements = privacyNoticePopup.FindElementsByClassName("Static");
            this.imageIcon = staticElements[0];
            this.privacyNoticeDetails = staticElements[1];
            this.yesButton = privacyNoticePopup.FindElementByName("Yes");
            this.noButton = privacyNoticePopup.FindElementByName("No");
        }

        /// <summary>
        /// Get the details on Privacy notice popup.
        /// </summary>
        /// <returns>The details on privacy notice.</returns>
        public string GetPrivacyNoticeDetails()
        {
            return this.privacyNoticeDetails.Text;
        }

        /// <summary>
        /// Click the Yes button.
        /// </summary>
        public void ClickYesButton()
        {
            this.yesButton.Click();
        }

        /// <summary>
        /// Click the No button.
        /// </summary>
        public void ClickNoButton()
        {
            this.noButton.Click();
        }

        /// <summary>
        /// Get the image icon on privacy notice popup.
        /// </summary>
        /// <returns>The image icon.</returns>
        public AppiumWebElement GetImageIcon()
        {
            return this.imageIcon;
        }
    }
}
