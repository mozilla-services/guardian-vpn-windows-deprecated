// <copyright file="SubscriptionSuccessPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for verification code page.
    /// </summary>
    internal class SubscriptionSuccessPage
    {
        private WindowsElement takeMeToProductLink;
        private WindowsDriver<WindowsElement> browserSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionSuccessPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public SubscriptionSuccessPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.browserSession = browserSession;
            this.takeMeToProductLink = Utils.WaitUntilFindElement(browserSession.FindElementByName, "No thanks, just take me to my product.");
        }

        /// <summary>
        /// Click verify button.
        /// </summary>
        public void ClickTakeMeToProductLink()
        {
            try
            {
                this.takeMeToProductLink.Click();
            }
            catch (Exception)
            {
                Utils.WaitUntilFindElement(this.browserSession.FindElementByName, "No thanks, just take me to my product.").Click();
            }
        }
    }
}
