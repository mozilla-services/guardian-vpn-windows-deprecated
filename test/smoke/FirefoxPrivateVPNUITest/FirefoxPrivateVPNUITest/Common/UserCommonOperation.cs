// <copyright file="UserCommonOperation.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Net;
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
            EmailInputPage emailInputPage = new EmailInputPage(browser.Session);
            emailInputPage.InputEmail(Environment.GetEnvironmentVariable("EXISTED_USER_NAME"));
            emailInputPage.ClickContinueButton();

            // Password Input Page
            PasswordInputPage passwordInputPage = new PasswordInputPage(browser.Session);
            passwordInputPage.InputPassword(Environment.GetEnvironmentVariable("EXISTED_USER_PASSWORD"));
            passwordInputPage.ClickSignInButton();

            // Quick Access Screen
            vpnClient.Session.SwitchTo();
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
            MainScreen mainScreen = new MainScreen(vpnClient.Session);
            if (mainScreen.GetOnImage().Displayed)
            {
                mainScreen.ToggleVPNSwitch();
            }

            mainScreen.ClickSettingsButton();
            SettingScreen settingScreen = new SettingScreen(vpnClient.Session);
            Assert.AreEqual("Settings", settingScreen.GetTitle());
            settingScreen.ScrollDown();
            settingScreen.ClickSignOutButton();
        }

        /// <summary>
        /// User click the toggle switch to connect to VPN.
        /// </summary>
        /// <param name="vpnClient">VPN session.</param>
        /// <param name="desktop">Desktop session.</param>
        public static void ConnectVPN(FirefoxPrivateVPNSession vpnClient, DesktopSession desktop)
        {
            // Click VPN switch toggle and turn on VPN
            vpnClient.Session.SwitchTo();
            MainScreen mainScreen = new MainScreen(vpnClient.Session);
            Assert.AreEqual("VPN is off", mainScreen.GetTitle());
            Assert.AreEqual("Turn it on to protect your entire device", mainScreen.GetSubtitle());
            Assert.IsTrue(mainScreen.GetOffImage().Displayed);
            Assert.IsFalse(mainScreen.GetOnImage().Displayed);
            mainScreen.ToggleVPNSwitch();

            // Verify the windows notification
            WindowsNotificationScreen windowsNotificationScreen = new WindowsNotificationScreen(desktop.Session);
            Assert.AreEqual("VPN is on", windowsNotificationScreen.GetTitleText());
            Assert.AreEqual("You're secure and protected.", windowsNotificationScreen.GetMessageText());
            windowsNotificationScreen.ClickDismissButton();

            // Verify user is connected to Mullvad VPN
            IRestResponse response = Utils.AmIMullvad("You are connected to Mullvad");
            Console.WriteLine($"After connection - Mullvad connected API response: {response.Content}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Contains("You are connected to Mullvad"));

            // Verify the changes on main screen
            vpnClient.Session.SwitchTo();
            Assert.IsTrue(mainScreen.GetOnImage().Displayed);
            Assert.IsFalse(mainScreen.GetOffImage().Displayed);
            Assert.AreEqual("VPN is on", mainScreen.GetTitle());
            Assert.IsTrue(mainScreen.GetSubtitle().Contains("Secure and protected"));
        }

        /// <summary>
        /// User click the toggle switch to turn off VPN.
        /// </summary>
        /// <param name="vpnClient">VPN session.</param>
        /// <param name="desktop">Desktop session.</param>
        public static void DisconnectVPN(FirefoxPrivateVPNSession vpnClient, DesktopSession desktop)
        {
            // Click VPN switch toggle and turn off VPN
            vpnClient.Session.SwitchTo();
            MainScreen mainScreen = new MainScreen(vpnClient.Session);
            mainScreen.ToggleVPNSwitch();

            // Verify the windows notification
            desktop.Session.SwitchTo();
            WindowsNotificationScreen windowsNotificationScreen = new WindowsNotificationScreen(desktop.Session);
            Assert.AreEqual("VPN is off", windowsNotificationScreen.GetTitleText());
            Assert.AreEqual("You disconnected.", windowsNotificationScreen.GetMessageText());
            windowsNotificationScreen.ClickDismissButton();

            vpnClient.Session.SwitchTo();
            Assert.IsTrue(mainScreen.GetOffImage().Displayed);
            Assert.IsFalse(mainScreen.GetOnImage().Displayed);
            Assert.AreEqual("VPN is off", mainScreen.GetTitle());
            Assert.AreEqual("Turn it on to protect your entire device", mainScreen.GetSubtitle());

            // Verify user disconnected to Mullvad VPN
            IRestResponse response = Utils.AmIMullvad("You are not connected to Mullvad");
            Console.WriteLine($"After disconnection - Mullvad connected API response: {response.Content}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Contains("You are not connected to Mullvad"));
        }
    }
}
