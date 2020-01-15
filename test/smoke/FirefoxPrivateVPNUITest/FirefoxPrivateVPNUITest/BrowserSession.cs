using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;

namespace FirefoxPrivateVPNUITest
{
    public class BrowserSession: IDisposable
    {
        // Note: append /wd/hub to the URL if you're directing the test at Appium
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string IEExplorerAppId = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge";

        public WindowsDriver<WindowsElement> session { get; }

        public BrowserSession()
        {
            if (session == null)
            {
                // Create a new session to bring up an instance of the FirefoxPrivateNetworkVPN application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", IEExplorerAppId);
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        public void Dispose()
        {
            // Close the application and delete the session
            if (session != null)
            {
                session.Close();
                session.Quit();
            }
        }
    }
}
