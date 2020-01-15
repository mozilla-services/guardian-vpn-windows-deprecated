using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;

namespace FirefoxPrivateVPNUITest
{
    public class FirefoxPrivateVPNSession: IDisposable
    {
        // Note: append /wd/hub to the URL if you're directing the test at Appium
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string FirefoxPrivateVPNAppId = @"C:\Program Files\Mozilla\Firefox Private Network VPN\FirefoxPrivateNetworkVPN.exe";

        public WindowsDriver<WindowsElement> Session;

        public FirefoxPrivateVPNSession()
        {
            if (Session == null)
            {
                // Create a new session to bring up an instance of the FirefoxPrivateNetworkVPN application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", FirefoxPrivateVPNAppId);
                appCapabilities.SetCapability("deviceName", "WindowsPC");
                Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(Session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        public void Dispose()
        {
            // Close the application and delete the session
            if (Session != null)
            {
                Session.Quit();
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", "Root");
                var desktopSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
                desktopSession.FindElementByName("Notification Chevron").Click();
                var clientTray = desktopSession.FindElementByName("Firefox Private Network VPN - Disconnected");
                desktopSession.Mouse.ContextClick(clientTray.Coordinates);
                desktopSession.FindElementByName("_Exit").Click();
                desktopSession.Quit();
                Session = null;
            }
        }
    }
}
