// <copyright file="LayoutScreenTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;

    /// <summary>
    /// This is to test the layout screen.
    /// </summary>
    [TestClass]
    public class LayoutScreenTest
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
        public void TestLayoutScreen()
        {
            this.vpnClient.Session.SwitchTo();

            // Test maximize
            this.vpnClient.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowUp + Keys.Command);
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Recover to normal size
            this.vpnClient.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowDown + Keys.Command);
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Test minimize
            this.vpnClient.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowDown + Keys.Command);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }
}
