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
        private AppiumWebElement profileImage;
        private AppiumWebElement userName;
        private AppiumWebElement manageAccountButton;
        private AppiumWebElement launchVPNStartupCheckbox;
        private AppiumWebElement networkSettingButton;
        private AppiumWebElement notificationButton;
        private AppiumWebElement languageButton;
        private AppiumWebElement aboutButton;
        private AppiumWebElement helpButton;
        private AppiumWebElement giveFeedbackText;
        private AppiumWebElement giveFeedbackLinkButton;
        private WindowsDriver<WindowsElement> vpnSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public SettingScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            this.vpnSession = vpnSession;
            var settingView = Utils.WaitUntilFindElement(vpnSession.FindElementByClassName, "SettingsView");
            this.backButton = Utils.WaitUntilFindElement(settingView.FindElementByName, "Back");
            this.titleText = Utils.WaitUntilFindElement(settingView.FindElementByName, "Settings");
            this.signOutButton = Utils.WaitUntilFindElement(settingView.FindElementByName, "Sign out");
            this.scrollDownButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "PART_LineDownButton");
            this.profileImage = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "ProfileImageButton");
            var scrollViewer = Utils.WaitUntilFindElement(settingView.FindElementByClassName, "ScrollViewer");
            this.userName = Utils.WaitUntilFindElements(scrollViewer.FindElementsByClassName, "TextBlock")[1];
            this.manageAccountButton = Utils.WaitUntilFindElement(settingView.FindElementByName, "Manage account");
            this.launchVPNStartupCheckbox = Utils.WaitUntilFindElement(settingView.FindElementByName, "Launch VPN app on computer startup");
            this.notificationButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "NotificationsNavButton");
            this.networkSettingButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "NetworkSettingsNavButton");
            this.languageButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "LanguageNavButton");
            this.aboutButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "AboutNavButton");
            this.helpButton = Utils.WaitUntilFindElement(settingView.FindElementByAccessibilityId, "GetHelpNavButton");
            this.giveFeedbackText = Utils.WaitUntilFindElement(settingView.FindElementByName, "Give feedback");
            this.giveFeedbackLinkButton = Utils.WaitUntilFindElement(settingView.FindElementByName, "Open link");
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
        /// Scroll down to show other element when necessary.
        /// </summary>
        public void ScrollDown()
        {
            this.scrollDownButton.Click();
            this.vpnSession.Mouse.MouseDown(null);

            // Scroll down for 2 second to show signout button
            Thread.Sleep(TimeSpan.FromSeconds(2));
            this.vpnSession.Mouse.MouseUp(null);
        }

        /// <summary>
        /// Click the Sign Out button.
        /// </summary>
        public void ClickSignOutButton()
        {
            this.signOutButton.Click();
        }

        /// <summary>
        /// Get the profile image element.
        /// </summary>
        /// <returns>The profile image element.</returns>
        public AppiumWebElement GetProfileImage()
        {
            return this.profileImage;
        }

        /// <summary>
        /// Get the user name.
        /// </summary>
        /// <returns>User name.</returns>
        public string GetUserName()
        {
            return this.userName.Text;
        }

        /// <summary>
        /// Click the Manage account button.
        /// </summary>
        public void ClickManageAccountButton()
        {
            this.manageAccountButton.Click();
        }

        /// <summary>
        /// Get the text on Launch VPN on computer startup.
        /// </summary>
        /// <returns>The text on Launch VPN on computer startup.</returns>
        public string GetLaunchVPNStartupText()
        {
            return this.launchVPNStartupCheckbox.Text;
        }

        /// <summary>
        /// Click the launch vpn startup checkbox.
        /// </summary>
        public void ClickLaunchVPNStartupCheckbox()
        {
            this.launchVPNStartupCheckbox.Click();
        }

        /// <summary>
        /// Click the notification button.
        /// </summary>
        public void ClickNotificationButton()
        {
            this.notificationButton.Click();
        }

        /// <summary>
        /// Click the network setting button.
        /// </summary>
        public void ClickNetworkSettingButton()
        {
            this.networkSettingButton.Click();
        }

        /// <summary>
        /// Click the language button.
        /// </summary>
        public void ClickLanguageButton()
        {
            this.languageButton.Click();
        }

        /// <summary>
        /// Click the about button.
        /// </summary>
        public void ClickAboutButton()
        {
            this.aboutButton.Click();
        }

        /// <summary>
        /// Click the Get Help button.
        /// </summary>
        public void ClickGetHelpButton()
        {
            this.helpButton.Click();
        }

        /// <summary>
        /// Click the Give feedback link.
        /// </summary>
        public void ClickGiveFeedbackLink()
        {
            this.giveFeedbackLinkButton.Click();
        }

        /// <summary>
        /// Get the text on notification button.
        /// </summary>
        /// <returns>The text on notification button.</returns>
        public string GetNotificationButtonText()
        {
            return this.notificationButton.Text;
        }

        /// <summary>
        /// Get the text on network setting button.
        /// </summary>
        /// <returns>The text on network setting button.</returns>
        public string GetNetworkSettingButtonText()
        {
            return this.networkSettingButton.Text;
        }

        /// <summary>
        /// Get the text on language button.
        /// </summary>
        /// <returns>The text on language button.</returns>
        public string GetLanguageButtonText()
        {
            return this.languageButton.Text;
        }

        /// <summary>
        /// Get the text on about button.
        /// </summary>
        /// <returns>The text on about button.</returns>
        public string GetAboutButtonText()
        {
            return this.aboutButton.Text;
        }

        /// <summary>
        /// Get the text on GetHelp button.
        /// </summary>
        /// <returns>The text on GetHelp button.</returns>
        public string GetHelpButtonText()
        {
            return this.helpButton.Text;
        }

        /// <summary>
        /// Get GiveFeedback text.
        /// </summary>
        /// <returns>GiveFeedback text.</returns>
        public string GetGiveFeedbackText()
        {
            return this.giveFeedbackText.Text;
        }

        /// <summary>
        /// Get Manage account button text.
        /// </summary>
        /// <returns>Manage account button text.</returns>
        public string GetManageAccountButtonText()
        {
            return this.manageAccountButton.Text;
        }

        /// <summary>
        /// Get Launch VPN startup checkbox.
        /// </summary>
        /// <returns>The Launch VPN startup checkbox.</returns>
        public AppiumWebElement GetLaunchVPNStartupCheckBox()
        {
            return this.launchVPNStartupCheckbox;
        }
    }
}
