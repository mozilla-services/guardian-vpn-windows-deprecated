// <copyright file="NewUserSignInTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This Sign In test is for new users.
    /// </summary>
    [TestClass]
    [Ignore]
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
        public void TestSignInFlow()
        {
            // Switch to VPN client session
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            landingScreen.ClickGetStartedButton();

            // Switch to Browser session
            this.browser.Session.SwitchTo();
            EmailInputPage loginScreen = new EmailInputPage(this.browser.Session);
            loginScreen.InputEmail("mhuang+123456@connected.io");
            loginScreen.ClickContinueButton();
            RegisterPage registerPage = new RegisterPage(this.browser.Session);
            registerPage.InputPassword("Passw0rd!");
            registerPage.InputRepeatPassword("Passw0rd!");
            registerPage.InputAge("30");
            registerPage.ClickCreateAccountButton();

            // TODO: need to insert a predicatable verification code
            // TODO: set up subscription payment
            // TODO: switch back to VPN client and click continue
            // TODO: Sign out
        }
    }
}
