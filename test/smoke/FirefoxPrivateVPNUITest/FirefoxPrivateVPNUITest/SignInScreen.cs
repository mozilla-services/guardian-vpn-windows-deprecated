using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace FirefoxPrivateVPNUITest
{
    [TestClass]
    public class SignInScreen
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;

        [TestInitialize]
        public void TestInitialize()
        {
            vpnClient = new FirefoxPrivateVPNSession();
            browser = new BrowserSession();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            vpnClient.Dispose();
            browser.Dispose();
        }

        [TestMethod]
        public void TestSignInFlow()
        {

        }
    }
}
