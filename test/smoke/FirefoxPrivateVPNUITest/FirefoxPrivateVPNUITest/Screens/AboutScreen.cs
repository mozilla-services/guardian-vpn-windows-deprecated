// <copyright file="AboutScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for About screen.
    /// </summary>
    internal class AboutScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement fpnTitle;
        private AppiumWebElement fpnSubtitle;
        private AppiumWebElement releaseTitle;
        private AppiumWebElement releaseVersion;
        private AppiumWebElement termsOfService;
        private AppiumWebElement termsOfServiceButton;
        private AppiumWebElement privacyPolicy;
        private AppiumWebElement privacyPolicyButton;
        private AppiumWebElement debug;
        private AppiumWebElement debugButton;
        private AppiumWebElement viewLog;
        private AppiumWebElement viewLogButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public AboutScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var aboutView = vpnSession.FindElementByClassName("AboutView");
            this.backButton = aboutView.FindElementByName("Back");
            this.title = aboutView.FindElementByName("About");
            var textBlocks = aboutView.FindElementsByClassName("TextBlock");
            this.fpnTitle = textBlocks[1];
            this.fpnSubtitle = textBlocks[2];
            this.releaseTitle = textBlocks[3];
            this.releaseVersion = textBlocks[4];
            this.termsOfService = aboutView.FindElementByName("Terms of Service");
            this.termsOfServiceButton = aboutView.FindElementByName("Open Terms of Service link");
            this.privacyPolicy = aboutView.FindElementByName("Privacy Policy");
            this.privacyPolicyButton = aboutView.FindElementByName("Open Privacy Policy link");
            this.debug = aboutView.FindElementByName("Debug");
            this.debugButton = aboutView.FindElementByName("Open Debug link");
            this.viewLog = aboutView.FindElementByName("View Log");
            this.viewLogButton = aboutView.FindElementByName("Open View log link");
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
        /// Get FPN title on about screen.
        /// </summary>
        /// <returns>The FPN title.</returns>
        public string GetFPNTitle()
        {
            return this.fpnTitle.Text;
        }

        /// <summary>
        /// Get FPN subtitle on about screen.
        /// </summary>
        /// <returns>The FPN subtitle.</returns>
        public string GetFPNSubtitle()
        {
            return this.fpnSubtitle.Text;
        }

        /// <summary>
        /// Get release title.
        /// </summary>
        /// <returns>The release title.</returns>
        public string GetReleaseTitle()
        {
            return this.releaseTitle.Text;
        }

        /// <summary>
        /// Get release version.
        /// </summary>
        /// <returns>The release version.</returns>
        public string GetReleaseVersion()
        {
            return this.releaseVersion.Text;
        }

        /// <summary>
        /// Get terms of service.
        /// </summary>
        /// <returns>Terms of service.</returns>
        public string GetTermsOfService()
        {
            return this.termsOfService.Text;
        }

        /// <summary>
        /// Get privacy policy.
        /// </summary>
        /// <returns>The privacy policy.</returns>
        public string GetPrivacyPolicy()
        {
            return this.privacyPolicy.Text;
        }

        /// <summary>
        /// Get Debug.
        /// </summary>
        /// <returns>The debug.</returns>
        public string GetDebug()
        {
            return this.debug.Text;
        }

        /// <summary>
        /// Get view log.
        /// </summary>
        /// <returns>The view log.</returns>
        public string GetViewLog()
        {
            return this.viewLog.Text;
        }

        /// <summary>
        /// Click terms of service button.
        /// </summary>
        public void ClickTermsOfService()
        {
            this.termsOfServiceButton.Click();
        }

        /// <summary>
        /// Click privacy policy button.
        /// </summary>
        public void ClickPrivacyPolicy()
        {
            this.privacyPolicyButton.Click();
        }

        /// <summary>
        /// Click Debug button.
        /// </summary>
        public void ClickDebug()
        {
            this.debugButton.Click();
        }

        /// <summary>
        /// Click view log button.
        /// </summary>
        public void ClickViewLog()
        {
            this.viewLogButton.Click();
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
