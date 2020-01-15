using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace FirefoxPrivateVPNUITest
{
    [TestClass]
    public class LayoutScreen
    {
        private FirefoxPrivateVPNSession vpnClient;

        [TestInitialize]
        public void TestInitialize()
        {
            vpnClient = new FirefoxPrivateVPNSession();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            vpnClient.Dispose();
        }
        [TestMethod]
        public void TestLayoutScreen()
        {
            // Test minimize
            vpnClient.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowDown + Keys.Command);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            // Test maximize
            vpnClient.Session.Keyboard.SendKeys(Keys.Command + Keys.ArrowUp + Keys.ArrowUp + Keys.Command);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }
}
