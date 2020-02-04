// <copyright file="BrowserSession.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// Firefox Browser session.
    /// </summary>
    public class BrowserSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string BrowserAppId = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserSession"/> class.
        /// </summary>
        public BrowserSession()
        {
            if (this.Session == null)
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();
                capabilities.SetCapability("platormName", "Windows");
                capabilities.SetCapability("deviceName", "WindowsPC");
                capabilities.SetCapability("app", BrowserAppId);
                try
                {
                    this.Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);
                    Assert.IsNotNull(this.Session);
                }
                catch (Exception)
                {
                    // 1. Creating a Desktop session
                    DesiredCapabilities desktopAppCapabilities = new DesiredCapabilities();
                    desktopAppCapabilities.SetCapability("app", "Root");
                    var desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), desktopAppCapabilities);
                    bool retry = true;
                    int retryTimes = 0;
                    IWebElement firefoxWindows = null;
                    while (retry)
                    {
                        retryTimes += 1;
                        WebDriverWait wait = new WebDriverWait(desktopSession, TimeSpan.FromSeconds(600));
                        firefoxWindows = wait.Until(ExpectedConditions.ElementExists(By.ClassName("MozillaWindowClass")));
                        if (firefoxWindows == null)
                        {
                            retry = true;
                            if (retryTimes == 5)
                            {
                                throw new Exception("Unable to launch firefox browser");
                            }
                        }
                        else
                        {
                            retry = false;
                        }
                    }

                    // 2. Attaching to existing firefox Window
                    string applicationSessionHandle = firefoxWindows.GetAttribute("NativeWindowHandle");
                    applicationSessionHandle = int.Parse(applicationSessionHandle).ToString("x");
                    DesiredCapabilities appCapabilities = new DesiredCapabilities();
                    appCapabilities.SetCapability("deviceName", "WindowsPC");
                    appCapabilities.SetCapability("appTopLevelWindow", applicationSessionHandle);
                    this.Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                }

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                this.Session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
            }
        }

        /// <summary>
        /// Gets or sets Session.
        /// </summary>
        public WindowsDriver<WindowsElement> Session { get; set; }

        /// <summary>
        /// Dispose the browser session and close the browser.
        /// </summary>
        public void Dispose()
        {
            // Close the application and delete the session
            if (this.Session != null)
            {
                this.Session.Close();
                WindowsElement closeButton = this.Session.FindElementByName("Close Tabs");
                closeButton.Click();
                this.Session.Quit();
                this.Session = null;
            }
        }
    }
}
