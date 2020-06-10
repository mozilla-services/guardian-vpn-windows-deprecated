// <copyright file="SettingTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is to test setting screen.
    /// </summary>
    [TestClass]
    public class SettingTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;
        private DesktopSession desktop;
        private string folderPath;
        private string debugFileName;
        private string logFileName;

        /// <summary>
        /// Initialize browser, vpn client, desktop sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.folderPath = "C:/Temp";
            this.debugFileName = "test.zip";
            this.logFileName = "test.txt";
            if (!Directory.Exists(this.folderPath))
            {
                Directory.CreateDirectory(this.folderPath);
            }

            // check the file exists or not. If already existed then delete it.
            foreach (string fileName in new List<string> { this.debugFileName, this.logFileName })
            {
                string fullPath = Path.Combine(this.folderPath, fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
            this.desktop = new DesktopSession();
            Utils.RearrangeWindows(this.vpnClient, this.browser);
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
            UserCommonOperation.UserSignIn(this.vpnClient, this.browser);

            // Main Screen
            this.vpnClient.Session.SwitchTo();
            MainScreen mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ClickSettingsButton();

            // Setting Screen
            SettingScreen settingScreen = new SettingScreen(this.vpnClient.Session);
            Assert.IsNotNull(settingScreen.GetProfileImage());
            Assert.AreEqual(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"), settingScreen.GetUserName());
            Assert.AreEqual("Manage account", settingScreen.GetManageAccountButtonText());
            Assert.AreEqual("Launch VPN app on computer startup", settingScreen.GetLaunchVPNStartupText());
            Assert.AreEqual("Notifications", settingScreen.GetNotificationButtonText());
            Assert.AreEqual("Network settings", settingScreen.GetNetworkSettingButtonText());
            Assert.AreEqual("Language", settingScreen.GetLanguageButtonText());
            Assert.AreEqual("About", settingScreen.GetAboutButtonText());
            Assert.AreEqual("Get help", settingScreen.GetHelpButtonText());
            Assert.AreEqual("Give feedback", settingScreen.GetGiveFeedbackText());
            Assert.IsTrue(settingScreen.GetLaunchVPNStartupCheckBox().Enabled);

            // Click Manage Account button
            settingScreen.ClickManageAccountButton();
            this.browser.Session.SwitchTo();
            Assert.IsTrue(this.browser.GetCurrentUrl().Contains(Constants.ManageAccountUrl));

            // Test checkbox state remain after nav
            bool prevLaunchVPNStartupState = settingScreen.GetLaunchVPNStartupCheckBox().Selected;
            settingScreen.ClickLaunchVPNStartupCheckbox();
            bool expectedCurrentLaunchVPNStartupState = !prevLaunchVPNStartupState;

            // Nav back to main screen
            settingScreen.ClickBackButton();

            // nav back to setting screen again to check the state remaining the same
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ClickSettingsButton();

            // Verify the state
            settingScreen = new SettingScreen(this.vpnClient.Session);
            bool currentLaunchVPNStartupState = settingScreen.GetLaunchVPNStartupCheckBox().Selected;
            Assert.AreEqual(expectedCurrentLaunchVPNStartupState, currentLaunchVPNStartupState);

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

            // Nav to Network Settings screen
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickNetworkSettingButton();

            // On network settings screen
            NetworkSettingsScreen networkSettingsScreen = new NetworkSettingsScreen(this.vpnClient.Session);
            bool preEnableIPv6 = networkSettingsScreen.IsEnableIPv6Checked();
            bool preAllowAccess = networkSettingsScreen.IsAllowAccessChecked();
            Assert.AreEqual("Network settings", networkSettingsScreen.GetTitle());
            Assert.AreEqual("Enable IPv6", networkSettingsScreen.GetEnableIPv6CheckBoxText());
            Assert.AreEqual("Push the internet forward with the latest version of the Internet Protocol", networkSettingsScreen.GetEnableIPv6Description());
            Assert.IsFalse(networkSettingsScreen.IsEnableIPv6DisabledMessageDisplayed());
            Assert.AreEqual("Local network access", networkSettingsScreen.GetAllowAccessText());
            Assert.AreEqual("Access printers, streaming sticks and all other devices on your local network", networkSettingsScreen.GetAllowAccessDescription());
            Assert.IsFalse(networkSettingsScreen.IsAllowAccessDisabledMessageDisplayed());

            // check the checkbox state after click
            networkSettingsScreen.ClickAllowAccessCheckBox();
            networkSettingsScreen.ClickEnableIPv6CheckBox();
            bool expectedEnableIPv6 = !preEnableIPv6;
            bool expectedAllowAccess = !preAllowAccess;
            networkSettingsScreen.ClickBackButton();

            // Back to main screen to turn on vpn
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickBackButton();
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ToggleVPNSwitch();
            mainScreen.ClickSettingsButton();

            // Check Network setting screen again
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ScrollDown();
            settingScreen.ClickNetworkSettingButton();
            networkSettingsScreen = new NetworkSettingsScreen(this.vpnClient.Session);
            bool currentAllowAccessState = networkSettingsScreen.IsAllowAccessChecked();
            bool currentEnableIPv6 = networkSettingsScreen.IsEnableIPv6Checked();
            Assert.AreEqual(expectedAllowAccess, currentAllowAccessState);
            Assert.AreEqual(expectedEnableIPv6, currentEnableIPv6);
            Assert.IsFalse(networkSettingsScreen.IsAllowAccessEnabled());
            Assert.IsTrue(networkSettingsScreen.IsAllowAccessDisabledMessageDisplayed());
            Assert.AreEqual($"VPN must be off before {(currentAllowAccessState ? "disabling" : "enabling")}", networkSettingsScreen.GetAllowAccessDisabledMessage());
            Assert.IsFalse(networkSettingsScreen.IsEnableIPv6Enabled());
            Assert.IsTrue(networkSettingsScreen.IsEnableIPv6DisabledMessageDisplayed());
            Assert.AreEqual($"VPN must be off before {(currentEnableIPv6 ? "disabling" : "enabling")}", networkSettingsScreen.GetEnableIPv6DisabledMessage());
            networkSettingsScreen.ClickBackButton();

            // Back to main screen to turn off vpn
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickBackButton();
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ToggleVPNSwitch();
            mainScreen.ClickSettingsButton();

            // Nav to language screen
            settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ScrollDown();
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
            exportDebugPackageWindow.SaveFile(this.folderPath, this.debugFileName);
            Assert.IsTrue(Utils.WaitUntilFileExist(Path.Combine(this.folderPath, this.debugFileName)));

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
            saveLogWindow.SaveFile(this.folderPath, this.logFileName);
            Assert.IsTrue(Utils.WaitUntilFileExist(Path.Combine(this.folderPath, this.logFileName)));

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
