// <copyright file="ExistedUserSignIn.cs" company="Mozilla">
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
    public class ExistedUserSignIn
    {
        /// <summary>
        /// The existed user sign in steps.
        /// </summary>
        /// <param name="vpnClient">VPN session.</param>
        /// <param name="browser">Browser session.</param>
        public static void ExistedUserSignInFlow(FirefoxPrivateVPNSession vpnClient, BrowserSession browser)
        {
            // Switch to Browser session
            browser.Session.SwitchTo();

            // Email Input page
            EmailInputPage emailInputPage = new EmailInputPage(browser.Session);
            emailInputPage.InputEmail(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"));
            emailInputPage.ClickContinueButton();

            // Password Input Page
            Thread.Sleep(TimeSpan.FromSeconds(5));
            PasswordInputPage passwordInputPage = new PasswordInputPage(browser.Session);
            passwordInputPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            passwordInputPage.ClickSignInButton();
            browser.Dispose();

            // Quick Access Screen
            vpnClient.Session.SwitchTo();
            Thread.Sleep(TimeSpan.FromSeconds(5));
            QuickAccessScreen quickAccessScreen = new QuickAccessScreen(vpnClient.Session);
            Assert.AreEqual("Quick access", quickAccessScreen.GetTitle());
            Assert.AreEqual("You can quickly access Firefox Private Network from your taskbar tray", quickAccessScreen.GetSubTitle());
            Assert.AreEqual("Located next to the clock at the bottom right of your screen", quickAccessScreen.GetDescription());
            quickAccessScreen.ClickContinueButton();
        }
    }
}
