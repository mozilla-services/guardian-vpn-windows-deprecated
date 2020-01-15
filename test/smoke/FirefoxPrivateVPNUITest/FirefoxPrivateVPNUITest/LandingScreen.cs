using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace FirefoxPrivateVPNUITest
{
    [TestClass]
    public class LandingScreen
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
        public void TestLandingScreen()
        {
            // Assert the texts
            WindowsElement landingView = vpnClient.Session.FindElementByClassName("LandingView");
            var titles = landingView.FindElementsByClassName("TextBlock");
            Assert.IsTrue(titles.Count >= 2);
            Assert.AreEqual("Firefox Private Network", titles[0].Text);
            Assert.AreEqual("A fast, secure and easy to use VPN \r\n(Virtual Private Network).", titles[1].Text);
            // Assert the Get started button
            var getStartedButton = landingView.FindElementByName("Get started");
            Assert.AreEqual("Get started", getStartedButton.Text);
            // Assert the learn more hyperlink
            var learnMoreHyperlink = landingView.FindElementByName("Learn more");
            Assert.AreEqual("Learn more", learnMoreHyperlink.Text);
        }
    }
}
