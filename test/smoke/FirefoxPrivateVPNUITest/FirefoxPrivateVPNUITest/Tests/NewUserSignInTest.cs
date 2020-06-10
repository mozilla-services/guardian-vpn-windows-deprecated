// <copyright file="NewUserSignInTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This Sign In test is for new users.
    /// </summary>
    [TestClass]
    public class NewUserSignInTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;

        /// <summary>
        /// Intialize browser and vpn sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
            Utils.RearrangeWindows(this.vpnClient, this.browser);

            // Delete email from restmail.net
            Assert.IsTrue(Utils.DeleteUserFromRestMail(Constants.NewUserName));
        }

        /// <summary>
        /// Dispose both browser and vpn sessions.
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
        public void TestNewUserSignIn()
        {
            // Switch to VPN client session
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            landingScreen.ClickGetStartedButton();

            // Verify Account Screen
            this.vpnClient.Session.SwitchTo();
            VerifyAccountScreen verifyAccountScreen = new VerifyAccountScreen(this.vpnClient.Session);
            Assert.AreEqual("Waiting for sign in and subscription confirmation...", verifyAccountScreen.GetTitle());
            Assert.AreEqual("Cancel and try again", verifyAccountScreen.GetCancelTryAgainButtonText());

            // Switch to Browser session
            this.browser.Session.SwitchTo();

            // Email Input page
            EmailInputPage emailInputPage = new EmailInputPage(this.browser.Session);
            emailInputPage.InputEmail(Constants.NewUserEmail);
            emailInputPage.ClickContinueButton();

            // Registration password Input Page
            RegisterPage registerPage = new RegisterPage(this.browser.Session);
            registerPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            registerPage.InputRepeatPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            registerPage.InputAge("50");
            registerPage.ClickCreateAccountButton();

            // Get verification code from API
            string verificationCode = Utils.GetVerificationCode(Constants.NewUserName);

            // Enter verification code page
            VerificationCodePage verificationCodePage = new VerificationCodePage(this.browser.Session);
            verificationCodePage.InputVerificationCode(verificationCode);
            verificationCodePage.ClickVerifyButton();

            // Enter subscription page
            string subscriptionPageUrl = string.Empty;
            Utils.WaitUntil(ref subscriptionPageUrl, (str) => this.browser.GetCurrentUrl(), null, (str) => str.Contains(Constants.KeyWordInPaymentUrl));
            SubscriptionPage subscriptionPage = new SubscriptionPage(this.browser.Session);
            subscriptionPage.InputFullName(Constants.NewUserName);
            subscriptionPage.InputCardNumber(Environment.GetEnvironmentVariable("NEW_USER_CARD_NUMBER"));
            subscriptionPage.InputExpDate(Environment.GetEnvironmentVariable("NEW_USER_CARD_EXP_DATE"));
            subscriptionPage.InputCVC(Environment.GetEnvironmentVariable("NEW_USER_CARD_CVC"));
            subscriptionPage.InputZipCode(Environment.GetEnvironmentVariable("NEW_USER_ZIP_CODE"));
            subscriptionPage.ClickAuthorizeCheckBox();
            subscriptionPage.ClickSubmitButton();

            // Subscription Success Page
            SubscriptionSuccessPage subscriptionSuccessPage = new SubscriptionSuccessPage(this.browser.Session);
            subscriptionSuccessPage.ClickTakeMeToProductLink();

            // Quick Access Screen
            this.vpnClient.Session.SwitchTo();
            QuickAccessScreen quickAccessScreen = new QuickAccessScreen(this.vpnClient.Session);
            Assert.AreEqual("Quick access", quickAccessScreen.GetTitle());
            Assert.AreEqual("You can quickly access Firefox Private Network from your taskbar tray", quickAccessScreen.GetSubTitle());
            Assert.AreEqual("Located next to the clock at the bottom right of your screen", quickAccessScreen.GetDescription());
            quickAccessScreen.ClickContinueButton();

            // On main screen
            MainScreen main = new MainScreen(this.vpnClient.Session);
            main.ClickSettingsButton();

            // On setting screen
            SettingScreen settingScreen = new SettingScreen(this.vpnClient.Session);
            settingScreen.ClickManageAccountButton();

            // Open an account page in browser
            this.browser.Session.SwitchTo();
            emailInputPage = new EmailInputPage(this.browser.Session);
            emailInputPage.InputEmail(Constants.NewUserEmail);
            emailInputPage.ClickContinueButton();

            // Go to password input page
            PasswordInputPage passwordInputPage = new PasswordInputPage(this.browser.Session);
            passwordInputPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            passwordInputPage.ClickSignInButton();

            // Go to account management page
            ManageAccountPage manageAccountPage = new ManageAccountPage(this.browser.Session);
            manageAccountPage.ClickDeleteButton();
            manageAccountPage.ConfirmDeleteAccount(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));

            // User sign out
            this.vpnClient.Session.SwitchTo();
            settingScreen.ScrollDown();
            settingScreen.ClickSignOutButton();
        }
    }
}
