// <copyright file="ConnectionTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Net;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is to test server selection.
    /// </summary>
    [TestClass]
    public class ConnectionTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;
        private DesktopSession desktop;

        /// <summary>
        /// Initialize vpn client, browser, and desktop sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();
            this.desktop = new DesktopSession();

            // Resize browser to make vpn client and browser are not overlapped
            var vpnClientPosition = this.vpnClient.Session.Manage().Window.Position;
            var vpnClientSize = this.vpnClient.Session.Manage().Window.Size;
            this.browser.SetWindowPosition(vpnClientPosition.X + vpnClientSize.Width, 0);
        }

        /// <summary>
        /// Dispose vpn session, browser and desktop sessions.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.vpnClient.Dispose();
            this.browser.Dispose();
            this.desktop.Dispose();
        }

        /// <summary>
        /// Test vpn connection.
        /// </summary>
        [TestMethod]
        public void TestVPNConnection()
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
            mainScreen.ClickServerListButton();

            // Server Screen
            ServerListScreen serverListScreen = new ServerListScreen(this.vpnClient.Session);
            serverListScreen.RandomSelectDifferentCityServer("Miami");
            string prevCity = serverListScreen.GetSelectedCity();
            Console.WriteLine("Before switching: the selected city is {0}", prevCity);

            // User turns on VPN
            UserCommonOperation.ConnectVPN(this.vpnClient, this.desktop);

            // Verify city via Mullvad API
            var cityResponse = Utils.GetCityViaMullvad(prevCity);
            Console.WriteLine($"Before switching (server connected) - Mullvad city API response: {cityResponse.Content}");
            Assert.AreEqual(HttpStatusCode.OK, cityResponse.StatusCode);
            Assert.IsTrue(prevCity.Contains(cityResponse.Content.Trim()));

            // Click the server button
            mainScreen = new MainScreen(this.vpnClient.Session);
            mainScreen.ClickServerListButton();

            // Select a random US server
            serverListScreen = new ServerListScreen(this.vpnClient.Session);
            serverListScreen.RandomSelectDifferentCityServer("Atlanta");
            string currentCity = serverListScreen.GetSelectedCity();
            Console.WriteLine("After switching: the selected city is {0}", currentCity);

            // Check the subtitle
            Assert.AreEqual(string.Format("From {0} to {1}", prevCity, currentCity), mainScreen.GetSubtitle());

            // Check the windows notification again
            this.desktop.Session.SwitchTo();
            WindowsNotificationScreen windowsNotificationScreen = new WindowsNotificationScreen(this.desktop.Session);
            Assert.AreEqual(string.Format("From {0} to {1}", prevCity, currentCity), windowsNotificationScreen.GetTitleText());
            Assert.AreEqual("You switched servers.", windowsNotificationScreen.GetMessageText());
            windowsNotificationScreen.ClickDismissButton();

            // Verify city via Mullvad API
            cityResponse = Utils.GetCityViaMullvad(currentCity);
            Console.WriteLine($"After switching (server connected) - Mullvad city API response: {cityResponse.Content}");
            Assert.AreEqual(HttpStatusCode.OK, cityResponse.StatusCode);
            Assert.IsTrue(currentCity.Contains(cityResponse.Content.Trim()));

            // User turns off VPN
            UserCommonOperation.DisconnectVPN(this.vpnClient, this.desktop);

            // Sign out
            UserCommonOperation.UserSignOut(this.vpnClient);
        }
    }
}
