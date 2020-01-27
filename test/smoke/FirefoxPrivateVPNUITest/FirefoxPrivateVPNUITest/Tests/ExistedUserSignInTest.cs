// <copyright file="ExistedUserSignInTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This Sign In test is for users who already registered and paid.
    /// </summary>
    [TestClass]
    public class ExistedUserSignInTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;

        /// <summary>
        /// Initialize browser and vpn client sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
        }

        /// <summary>
        /// Dispose both vpn and browser sessions.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.vpnClient.Dispose();
            this.browser.Dispose();
        }

        /// <summary>
        /// The test steps.
        /// </summary>
        [TestMethod]
        public void TestSignInFlow()
        {
            // Switch to VPN client session
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            landingScreen.ClickGetStartedButton();

            // Switch to Browser session
            this.browser.Session.SwitchTo();

            // Email Input page
            EmailInputPage emailInputPage = new EmailInputPage(this.browser.Session);
            emailInputPage.InputEmail(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"));
            emailInputPage.ClickContinueButton();

            // Password Input Page
            PasswordInputPage passwordInputPage = new PasswordInputPage(this.browser.Session);
            passwordInputPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            passwordInputPage.ClickSignInButton();
            this.browser.Dispose();

            // Quick Access Screen
            this.vpnClient.Session.SwitchTo();
            Thread.Sleep(TimeSpan.FromSeconds(5));
            QuickAccessScreen quickAccessScreen = new QuickAccessScreen(this.vpnClient.Session);
            Assert.AreEqual("Quick access", quickAccessScreen.GetTitle());
            Assert.AreEqual("You can quickly access Firefox Private Network from your taskbar tray", quickAccessScreen.GetSubTitle());
            Assert.AreEqual("Located next to the clock at the bottom right of your screen", quickAccessScreen.GetDescription());
            quickAccessScreen.ClickContinueButton();

            // Main Screen
            MainScreen mainScreen = new MainScreen(this.vpnClient.Session);
            Assert.AreEqual("VPN is off", mainScreen.GetTitle());
            mainScreen.ClickSettingsButton();

            // Setting Screen
            SettingScreen settingScreen = new SettingScreen(this.vpnClient.Session);
            Assert.AreEqual("Settings", settingScreen.GetTitle());
            settingScreen.ClickSignOutButton();
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}
