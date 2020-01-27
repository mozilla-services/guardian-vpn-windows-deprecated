// <copyright file="LandingScreenTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is to test the landing screen of the VPN client.
    /// </summary>
    [TestClass]
    public class LandingScreenTest
    {
        private FirefoxPrivateVPNSession vpnClient;

        /// <summary>
        /// Initialize the vpn session.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.vpnClient = new FirefoxPrivateVPNSession();
        }

        /// <summary>
        /// Dispose the vpn session.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.vpnClient.Dispose();
        }

        /// <summary>
        /// The test steps.
        /// </summary>
        [TestMethod]
        public void TestLandingScreen()
        {
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            Assert.AreEqual("Firefox Private Network", landingScreen.GetTitle());
            Assert.AreEqual("A fast, secure and easy to use VPN \r\n(Virtual Private Network).", landingScreen.GetSubTitle());
            Assert.AreEqual("Get started", landingScreen.GetStartedButtonText());
            Assert.AreEqual("Learn more", landingScreen.GetLearnMoreHyperlinkText());
        }
    }
}
