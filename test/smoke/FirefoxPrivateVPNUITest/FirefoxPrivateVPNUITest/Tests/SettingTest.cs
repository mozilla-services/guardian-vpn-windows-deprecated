// <copyright file="SettingTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium.Windows;
    using RestSharp;

    /// <summary>
    /// This is to test setting screen.
    /// </summary>
    [TestClass]
    public class SettingTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;
        private DesktopSession desktop;

        /// <summary>
        /// Initialize browser, vpn client, desktop sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // check the file exists or not. If already existed then delete it.
            foreach (string fileName in new List<string> { "test.zip", "test.txt" })
            {
                string fullPath = $"{Environment.CurrentDirectory}/{fileName}";
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
            this.desktop = new DesktopSession();

            // Resize browser to make vpn client and browser are not overlapped
            var vpnClientPosition = this.vpnClient.Session.Manage().Window.Position;
            var vpnClientSize = this.vpnClient.Session.Manage().Window.Size;
            this.browser.SetWindowPosition(vpnClientPosition.X + vpnClientSize.Width, 0);
        }

        /// <summary>
        /// Dispose vpn client, browser, desktop sessions.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.vpnClient.Dispose();
            this.browser.Dispose();
            this.desktop.Dispose();
        }

        /// <summary>
        /// The test steps.
        /// </summary>
        [TestMethod]
        public void TestSetting()
        {
            // Switch to VPN client session
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            landingScreen.ClickGetStartedButton();

            // User Sign In via web browser
            UserCommonOperation.UserSignIn(this.vpnClient, this.browser, false);

            // Main Screen
            this.vpnClient.Session.SwitchTo();
            MainScreen mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ClickSettingsButton();

            // Setting Screen
            SettingScreen settingScreen = new SettingScreen(this.vpnClient.Session);
            Assert.IsNotNull(settingScreen.GetProfileImage());
            Assert.AreEqual(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"), settingScreen.GetUserName());
            Assert.AreEqual("Manage account", settingScreen.GetManageAccountButtonText());
            Assert.AreEqual("Allow access to your local network", settingScreen.GetAllowAccessText());
            Assert.AreEqual("Access printers, streaming sticks and all other devices on your local network", settingScreen.GetAllowAccessDescription());
            Assert.IsFalse(settingScreen.GetAllowAccessDisabledMessage().Displayed);
            Assert.AreEqual("Launch VPN app on computer startup", settingScreen.GetLaunchVPNStartupText());
            Assert.AreEqual("Notifications", settingScreen.GetNotificationButtonText());
            Assert.AreEqual("Language", settingScreen.GetLanguageButtonText());
            Assert.AreEqual("About", settingScreen.GetAboutButtonText());
            Assert.AreEqual("Get help", settingScreen.GetHelpButtonText());
            Assert.AreEqual("Give feedback", settingScreen.GetGiveFeedbackText());
            Assert.IsTrue(settingScreen.GetAllowAccessCheckBox().Enabled);
            Assert.IsTrue(settingScreen.GetLaunchVPNStartupCheckBox().Enabled);

            // Click Manage Account button
            settingScreen.ClickManageAccountButton();
            this.browser.Session.SwitchTo();
            Assert.IsTrue(this.browser.GetCurrentUrl().Contains(Constants.ManageAccountUrl));

            // Test checkbox state remain after nav
            bool prevAllowAccessState = settingScreen.GetAllowAccessCheckBox().Selected;
            bool prevLaunchVPNStartupState = settingScreen.GetLaunchVPNStartupCheckBox().Selected;
            settingScreen.ClickAllowAccessCheckBox();
            settingScreen.ClickLaunchVPNStartupCheckbox();
            bool expectedCurrentAllowAccessState = !prevAllowAccessState;
            bool expectedCurrentLaunchVPNStartupState = !prevLaunchVPNStartupState;

            // Nav back to main screen
            settingScreen.ClickBackButton();

            // nav back to setting screen again to check the state remaining the same
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ToggleVPNSwitch();
            mainScreen.ClickSettingsButton();

            // Verify the state
            settingScreen = new SettingScreen(this.vpnClient.Session);
            bool currentAllowAccessState = settingScreen.GetAllowAccessCheckBox().Selected;
            bool currentLaunchVPNStartupState = settingScreen.GetLaunchVPNStartupCheckBox().Selected;
            Assert.AreEqual(expectedCurrentAllowAccessState, currentAllowAccessState);
            Assert.AreEqual(expectedCurrentLaunchVPNStartupState, currentLaunchVPNStartupState);
            Assert.IsFalse(settingScreen.GetAllowAccessCheckBox().Enabled);
            Assert.IsTrue(settingScreen.GetAllowAccessDisabledMessage().Displayed);
            Assert.AreEqual($"VPN must be off before {(currentAllowAccessState ? "disabling" : "enabling")}", settingScreen.GetAllowAccessDisabledMessage().Text);

            // Nav back to main screen and turn off vpn
            settingScreen.ClickBackButton();
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ToggleVPNSwitch();
            mainScreen.ClickSettingsButton();
            settingScreen = new SettingScreen(this.vpnClient.Session);

            // Click the notification button
            settingScreen.ScrollDown();
            settingScreen.ClickNotificationButton();
            NotificationsScreen notificationsScreen = new NotificationsScreen(this.vpnClient.Session);
            Assert.AreEqual("Notifications", notificationsScreen.GetTitle());
            Assert.AreEqual("Unsecured network alert", notificationsScreen.GetUnsecuredNetworkAlertText());
            Assert.AreEqual("Get notified if you connect to an unsecured Wi-Fi network", notificationsScreen.GetUnsecuredNetworkAlertDescription());
            Assert.AreEqual("Guest Wi-Fi portal alert", notificationsScreen.GetGuestWifiPortalAlertText());
            Assert.AreEqual("Get notified if a guest Wi-Fi portal is blocked due to VPN connection", notificationsScreen.GetGuestWifiPortalAlertDescription());

            // Check the state remain after nav
            bool prevUnsecuredNetworkAlertState = notificationsScreen.IsUnsecuredNetworkAlertChecked();
            bool prevGuestWifiPortalAlertState = notificationsScreen.IsGuestWifiPortalAlertChecked();
            notificationsScreen.ClickUnsecuredNetworkAlertCheckBox();
            notificationsScreen.ClickGuestWifiPortalAlertCheckBox();
            bool expectedCurrentUnsecuredNetworkAlertState = !prevUnsecuredNetworkAlertState;
            bool expectedCurrentGuestWifiPortalAlertState = !prevGuestWifiPortalAlertState;

            // Nav back to setting screen
            notificationsScreen.ClickBackButton();
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickNotificationButton();

            // Verifiy alert state
            notificationsScreen = new NotificationsScreen(this.vpnClient.Session);
            Assert.AreEqual(expectedCurrentUnsecuredNetworkAlertState, notificationsScreen.IsUnsecuredNetworkAlertChecked());
            Assert.AreEqual(expectedCurrentGuestWifiPortalAlertState, notificationsScreen.IsGuestWifiPortalAlertChecked());
            notificationsScreen.ClickBackButton();

            // Nav to language screen
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickLanguageButton();

            // On language screen
            LanguageScreen languageScreen = new LanguageScreen(this.vpnClient.Session);
            languageScreen.RandomPickAdditionalLanguage();

            // Reset back to English
            languageScreen.ClickDefaultLanguageRadioButton();
            languageScreen.ClickBackButton();

            // Nav to About screen
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickAboutButton();

            // About screen
            AboutScreen aboutScreen = new AboutScreen(this.vpnClient.Session);
            Assert.AreEqual("Firefox Private Network", aboutScreen.GetFPNTitle());
            Assert.AreEqual("A fast, secure and easy to use VPN \r\n(Virtual Private Network).", aboutScreen.GetFPNSubtitle());
            Assert.AreEqual("Release version", aboutScreen.GetReleaseTitle());
            Console.WriteLine($"Version: {aboutScreen.GetReleaseVersion()}");
            Assert.IsTrue(Regex.IsMatch(aboutScreen.GetReleaseVersion(), @"^\d+.\d+[abAB]?$"));

            // Click terms of service button
            this.vpnClient.Session.SwitchTo();
            aboutScreen.ClickTermsOfService();
            this.browser.Session.SwitchTo();
            string actualTermsOfServiceUrl = this.browser.GetCurrentUrl();
            Console.WriteLine($"Terms of service url: {actualTermsOfServiceUrl}");
            Assert.IsTrue(actualTermsOfServiceUrl.Contains(Constants.TermsOfServiceUrl));

            // Click privacy policy button
            this.vpnClient.Session.SwitchTo();
            aboutScreen.ClickPrivacyPolicy();
            this.browser.Session.SwitchTo();
            string actualPrivacyPolicyUrl = this.browser.GetCurrentUrl();
            Console.WriteLine($"Privacy Policy url: {actualPrivacyPolicyUrl}");
            Assert.IsTrue(actualPrivacyPolicyUrl.Contains(Constants.PrivacyPolicyUrl));

            // Click the Debug button
            this.vpnClient.Session.SwitchTo();
            aboutScreen.ClickDebug();

            // On Privacy Notice popup
            PrivacyNoticePopup privacyNoticePopup = new PrivacyNoticePopup(this.vpnClient.Session);
            Assert.AreEqual(Utils.CleanText(Constants.PrivacyNotice), Utils.CleanText(privacyNoticePopup.GetPrivacyNoticeDetails()));
            privacyNoticePopup.ClickYesButton();

            // Open Export Debug package window
            ExportWindow exportDebugPackageWindow = new ExportWindow(this.vpnClient.Session, "Export debug package");
            exportDebugPackageWindow.SaveFile(Environment.CurrentDirectory, "test.zip");
            Assert.IsTrue(Utils.WaitUntilFileExist($"{Environment.CurrentDirectory}/test.zip"));

            // Click viewlog button
            aboutScreen.ClickViewLog();
            LogWindow logWindow = new LogWindow(this.desktop.Session);
            Assert.AreEqual("Timestamp", logWindow.GetTimeStampColumnHeader());
            Assert.AreEqual("Message", logWindow.GetMessageColumnHeader());
            Assert.IsTrue(logWindow.GetNumberOfLogs() > 0);

            // Click save button on view log window
            logWindow.ClickSaveButton();

            // Open SaveLog window
            ExportWindow saveLogWindow = new ExportWindow(this.desktop.Session, "Save log");
            saveLogWindow.SaveFile(Environment.CurrentDirectory, "test.txt");
            Assert.IsTrue(Utils.WaitUntilFileExist($"{Environment.CurrentDirectory}/test.txt"));

            // Back to setting screen
            logWindow.CloseWindow();
            aboutScreen.ClickBackButton();

            // Click Get Help button
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickGetHelpButton();

            // On Get Help Screen
            GetHelpScreen getHelpScreen = new GetHelpScreen(this.vpnClient.Session);
            Assert.AreEqual("Get help", getHelpScreen.GetTitle());
            Assert.AreEqual("Contact us", getHelpScreen.GetContactUs());
            Assert.AreEqual("Help & Support", getHelpScreen.GetHelpAndSupport());

            // Click contact us button
            this.vpnClient.Session.SwitchTo();
            getHelpScreen.ClickContactUs();
            this.browser.Session.SwitchTo();
            string actualContactUsUrl = this.browser.GetCurrentUrl();
            Console.WriteLine($"Contact us url: {actualContactUsUrl}");
            Assert.IsTrue(actualContactUsUrl.Contains(Constants.ContactUsUrl));

            // Click help and support button
            this.vpnClient.Session.SwitchTo();
            getHelpScreen.ClickHelpAndSupport();
            this.browser.Session.SwitchTo();
            string actualHelpSupportUrl = this.browser.GetCurrentUrl();
            Console.WriteLine($"Help and support url: {actualHelpSupportUrl}");
            Assert.IsTrue(actualHelpSupportUrl.Contains(Constants.SupportUrl));

            // Back to setting screen
            this.vpnClient.Session.SwitchTo();
            getHelpScreen.ClickBackButton();

            // Click Give feedback button
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickGiveFeedbackLink();
            this.browser.Session.SwitchTo();
            Assert.IsTrue(this.browser.GetCurrentUrl().Contains(Constants.FeedbackUrl));

            // Sign out
            this.vpnClient.Session.SwitchTo();
            settingScreen.ClickSignOutButton();
        }
    }
}
