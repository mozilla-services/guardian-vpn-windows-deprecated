// <copyright file="DeviceTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium.Windows;
    using RestSharp;

    /// <summary>
    /// This is to test device screen.
    /// </summary>
    [TestClass]
    public class DeviceTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;

        /// <summary>
        /// Initialize browser, vpn client sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
        }

        /// <summary>
        /// Dispose vpn client, browser sessions.
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
        public void TestDevice()
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
            mainScreen.ClickDeviceListButton();

            // Device Screen
            DeviceScreen deviceScreen = new DeviceScreen(this.vpnClient.Session);
            Assert.AreEqual("My devices", deviceScreen.GetTitle());
            Regex rgx = new Regex(@"^[1-5] of 5$");
            Assert.IsTrue(rgx.IsMatch(deviceScreen.GetNumberOfDevice()));
            Assert.AreEqual("Devices with Firefox Private Network installed using your account. Connect up to 5 devices.", deviceScreen.GetDevicePanelTitle());
            Assert.IsTrue(deviceScreen.GetCurrentDeviceName().Contains(Environment.MachineName));
            Assert.AreEqual("Current device", deviceScreen.GetCurrentDeviceStatus());
            Assert.IsFalse(deviceScreen.GetCurrentDeviceRemoveButton().Displayed);
            deviceScreen.ClickBackButton();

            // Sign out
            UserCommonOperation.UserSignOut(this.vpnClient);
        }
    }
}
