// <copyright file="UserCommonOperation.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    /// <summary>
    /// Here are some common operations for users.
    /// </summary>
    public class UserCommonOperation
    {
        /// <summary>
        /// The user sign in steps.
        /// </summary>
        /// <param name="vpnClient">VPN session.</param>
        /// <param name="browser">Browser session.</param>
        public static void UserSignIn(FirefoxPrivateVPNSession vpnClient, BrowserSession browser)
        {
            // Verify Account Screen
            vpnClient.Session.SwitchTo();
            VerifyAccountScreen verifyAccountScreen = new VerifyAccountScreen(vpnClient.Session);
            Assert.AreEqual("Waiting for sign in and subscription confirmation...", verifyAccountScreen.GetTitle());
            Assert.AreEqual("Cancel and try again", verifyAccountScreen.GetCancelTryAgainButtonText());

            // Switch to Browser session
            browser.Session.SwitchTo();

            // Email Input page
            Thread.Sleep(TimeSpan.FromSeconds(2));
            EmailInputPage emailInputPage = new EmailInputPage(browser.Session);
            emailInputPage.InputEmail(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"));
            emailInputPage.ClickContinueButton();

            // Password Input Page
            Thread.Sleep(TimeSpan.FromSeconds(2));
            PasswordInputPage passwordInputPage = new PasswordInputPage(browser.Session);
            passwordInputPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            passwordInputPage.ClickSignInButton();
            browser.Dispose();

            // Quick Access Screen
            vpnClient.Session.SwitchTo();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            QuickAccessScreen quickAccessScreen = new QuickAccessScreen(vpnClient.Session);
            Assert.AreEqual("Quick access", quickAccessScreen.GetTitle());
            Assert.AreEqual("You can quickly access Firefox Private Network from your taskbar tray", quickAccessScreen.GetSubTitle());
            Assert.AreEqual("Located next to the clock at the bottom right of your screen", quickAccessScreen.GetDescription());
            quickAccessScreen.ClickContinueButton();
        }

        /// <summary>
        /// The user sign out steps.
        /// </summary>
        /// <param name="vpnClient">VPN session.</param>
        public static void UserSignOut(FirefoxPrivateVPNSession vpnClient)
        {
            vpnClient.Session.SwitchTo();
            SettingScreen settingScreen = new SettingScreen(vpnClient.Session);
            Assert.AreEqual("Settings", settingScreen.GetTitle());
            settingScreen.ClickSignOutButton();
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Send API request to check whether user is connected or not.
        /// </summary>
        /// <returns>The API response.</returns>
        public static IRestResponse AmIMullvad()
        {
            var client = new RestClient("https://am.i.mullvad.net/connected");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Host", "am.i.mullvad.net");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            return client.Execute(request);
        }
    }
}
