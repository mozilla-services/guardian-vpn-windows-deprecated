// <copyright file="ConnectTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Net;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    /// <summary>
    /// This is to test connection and disconnection functionality.
    /// </summary>
    [TestClass]
    public class ConnectTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;
        private DesktopSession desktop;

        /// <summary>
        /// Initialize browser, vpn client, and desktop session.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.desktop = new DesktopSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
        }

        /// <summary>
        /// Dispose vpn client, browser, and desktop sessions.
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
        public void TestConnection()
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
            Assert.AreEqual("VPN is off", mainScreen.GetTitle());
            Assert.AreEqual("Turn it on to protect your entire device", mainScreen.GetSubtitle());
            Assert.IsTrue(mainScreen.GetOffImage().Displayed);
            Assert.IsFalse(mainScreen.GetOnImage().Displayed);

            // User turns on VPN
            UserCommonOperation.ConnectVPN(this.vpnClient, this.desktop);

            // User turns off VPN
            UserCommonOperation.DisconnectVPN(this.vpnClient, this.desktop);

            // Sign out
            UserCommonOperation.UserSignOut(this.vpnClient);
        }
    }
}
