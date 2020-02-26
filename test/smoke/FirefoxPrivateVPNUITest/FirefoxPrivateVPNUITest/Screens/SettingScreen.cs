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
        private AppiumWebElement allowAccessCheckbox;
        private AppiumWebElement allowAccessDescription;
        private AppiumWebElement allowAccessDisabledMessage;
        private AppiumWebElement launchVPNStartupCheckbox;
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
            if (this.vpnSession == null)
            {
                this.vpnSession = vpnSession;
            }

            this.backButton = vpnSession.FindElementByName("Back");
            this.titleText = vpnSession.FindElementByName("Settings");
            this.signOutButton = vpnSession.FindElementByName("Sign out");
            this.scrollDownButton = vpnSession.FindElementByAccessibilityId("PART_LineDownButton");
            this.profileImage = vpnSession.FindElementByAccessibilityId("ProfileImageButton");
            this.userName = vpnSession.FindElementByName(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"));
            this.manageAccountButton = vpnSession.FindElementByName("Manage account");
            this.allowAccessCheckbox = vpnSession.FindElementByAccessibilityId("AllowLocalDeviceAccessCheckBox");
            this.allowAccessDescription = vpnSession.FindElementByName("Access printers, streaming sticks and all other devices on your local network");
            this.allowAccessDisabledMessage = vpnSession.FindElementByAccessibilityId("AllowLocalDeviceAccessCheckBoxDisabledMessage");
            this.launchVPNStartupCheckbox = vpnSession.FindElementByName("Launch VPN app on computer startup");
            this.notificationButton = vpnSession.FindElementByAccessibilityId("NotificationsNavButton");
            this.languageButton = vpnSession.FindElementByAccessibilityId("LanguageNavButton");
            this.aboutButton = vpnSession.FindElementByAccessibilityId("AboutNavButton");
            this.helpButton = vpnSession.FindElementByAccessibilityId("GetHelpNavButton");
            this.giveFeedbackText = vpnSession.FindElementByName("Give feedback");
            this.giveFeedbackLinkButton = vpnSession.FindElementByName("Open link");
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
        /// Get the text on Allow access checkbox.
        /// </summary>
        /// <returns>The text on allow access checkbox.</returns>
        public string GetAllowAccessText()
        {
            return this.allowAccessCheckbox.Text;
        }

        /// <summary>
        /// Get the description of the allow access checkbox.
        /// </summary>
        /// <returns>The description of the allow access checkbox.</returns>
        public string GetAllowAccessDescription()
        {
            return this.allowAccessDescription.Text;
        }

        /// <summary>
        /// Click the Allow Access check box.
        /// </summary>
        public void ClickAllowAccessCheckBox()
        {
            this.allowAccessCheckbox.Click();
        }

        /// <summary>
        /// Get the Allow access disabled message element.
        /// </summary>
        /// <returns>The Allow access disabled message element.</returns>
        public AppiumWebElement GetAllowAccessDisabledMessage()
        {
            return this.allowAccessDisabledMessage;
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
        /// Get Allow access checkbox.
        /// </summary>
        /// <returns>The allow access checkbox.</returns>
        public AppiumWebElement GetAllowAccessCheckBox()
        {
            return this.allowAccessCheckbox;
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
