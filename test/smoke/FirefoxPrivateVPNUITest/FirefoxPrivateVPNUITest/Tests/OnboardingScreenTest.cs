// <copyright file="OnboardingScreenTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is to test onboarding screen.
    /// </summary>
    [TestClass]
    public class OnboardingScreenTest
    {
        private FirefoxPrivateVPNSession vpnClient;
        private BrowserSession browser;

        /// <summary>
        /// Initialize vpn client and browser sessions.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.browser = new BrowserSession();
            this.vpnClient = new FirefoxPrivateVPNSession();

            // Resize browser to make vpn client and browser are not overlapped
            var vpnClientPosition = this.vpnClient.Session.Manage().Window.Position;
            var vpnClientSize = this.vpnClient.Session.Manage().Window.Size;
            this.browser.SetWindowPosition(vpnClientPosition.X + vpnClientSize.Width, 0);
        }

        /// <summary>
        /// Dispose vpn session.
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
        public void TestOnboarding()
        {
            // Switch to VPN client session
            this.vpnClient.Session.SwitchTo();
            LandingScreen landingScreen = new LandingScreen(this.vpnClient.Session);
            landingScreen.ClickLearnMoreHyperlink();

            // Go to onboarding screen 1
            OnboardingScreen onboardingScreen1 = new OnboardingScreen(this.vpnClient.Session, "OnboardingView1");
            Assert.AreEqual("Skip", onboardingScreen1.GetSkipText());
            Assert.AreEqual("Next", onboardingScreen1.GetNextButtonText());
            Assert.AreEqual("No activity logs", onboardingScreen1.GetTitle());
            Assert.AreEqual("We're Mozilla. We don't log your activity and we're always on your side.", onboardingScreen1.GetSubTitle());
            onboardingScreen1.ClickNextButton();

            // Go to onboarding screen 2
            OnboardingScreen onboardingScreen2 = new OnboardingScreen(this.vpnClient.Session, "OnboardingView2");
            Assert.AreEqual("Skip", onboardingScreen2.GetSkipText());
            Assert.AreEqual("Next", onboardingScreen2.GetNextButtonText());
            Assert.AreEqual("Device level encryption", onboardingScreen2.GetTitle());
            Assert.AreEqual("No one will see your location or activity, even on unsecure Wi-Fi networks.", onboardingScreen2.GetSubTitle());
            onboardingScreen2.ClickNextButton();

            // Go to onboarding screen 3
            OnboardingScreen onboardingScreen3 = new OnboardingScreen(this.vpnClient.Session, "OnboardingView3");
            Assert.AreEqual("Skip", onboardingScreen3.GetSkipText());
            Assert.AreEqual("Next", onboardingScreen3.GetNextButtonText());
            Assert.AreEqual("Servers in 39 countries", onboardingScreen3.GetTitle());
            Assert.AreEqual("Stand up to tech bullies and protect your access to the web.", onboardingScreen3.GetSubTitle());
            onboardingScreen3.ClickNextButton();

            // Go to onboarding screen 4
            LastOnboardingScreen onboardingScreen4 = new LastOnboardingScreen(this.vpnClient.Session);
            Assert.AreEqual("Get started", onboardingScreen4.GetGetStartedButtonText());
            Assert.AreEqual("Connect up to 5 devices", onboardingScreen4.GetTitle());
            Assert.AreEqual("Stream, download and game. We won't restrict your bandwidth.", onboardingScreen4.GetSubTitle());
            onboardingScreen4.ClickGetStartedButton();

            // User Sign In via web browser
            UserCommonOperation.UserSignIn(this.vpnClient, this.browser);

            // Main Screen
            this.vpnClient.Session.SwitchTo();
            MainScreen mainScreen = new MainScreen(this.vpnClient.Session);
            Assert.AreEqual("VPN is off", mainScreen.GetTitle());

            // Setting Screen
            UserCommonOperation.UserSignOut(this.vpnClient);
        }
    }
}
