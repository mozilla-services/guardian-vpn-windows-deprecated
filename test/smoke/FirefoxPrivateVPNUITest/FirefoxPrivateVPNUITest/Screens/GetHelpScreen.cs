// <copyright file="GetHelpScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for About screen.
    /// </summary>
    internal class GetHelpScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement contactUs;
        private AppiumWebElement contactUsButton;
        private AppiumWebElement helpSupport;
        private AppiumWebElement helpSupportButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetHelpScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public GetHelpScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var getHelpView = vpnSession.FindElementByClassName("GetHelpView");
            this.backButton = getHelpView.FindElementByName("Back");
            this.title = getHelpView.FindElementByName("Get help");
            this.contactUs = getHelpView.FindElementByName("Contact us");
            this.contactUsButton = getHelpView.FindElementByName("Open Contact us link");
            this.helpSupport = getHelpView.FindElementByName("Help & Support");
            this.helpSupportButton = getHelpView.FindElementByName("Open Help & Support link");
        }

        /// <summary>
        /// Get title on Language screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get contact us text.
        /// </summary>
        /// <returns>Contact us text.</returns>
        public string GetContactUs()
        {
            return this.contactUs.Text;
        }

        /// <summary>
        /// Get help & support text.
        /// </summary>
        /// <returns>The help & support text.</returns>
        public string GetHelpAndSupport()
        {
            return this.helpSupport.Text;
        }

        /// <summary>
        /// Click contact us button.
        /// </summary>
        public void ClickContactUs()
        {
            this.contactUsButton.Click();
        }

        /// <summary>
        /// Click help & support button.
        /// </summary>
        public void ClickHelpAndSupport()
        {
            this.helpSupportButton.Click();
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }
    }
}
