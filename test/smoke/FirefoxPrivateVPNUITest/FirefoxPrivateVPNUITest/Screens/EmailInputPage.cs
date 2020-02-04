// <copyright file="EmailInputPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This screen model is for email input page.
    /// </summary>
    internal class EmailInputPage
    {
        private WindowsElement emailTextBox;
        private WindowsElement continueButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailInputPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public EmailInputPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.emailTextBox = browserSession.FindElementByName("Email");
            this.continueButton = browserSession.FindElementByName("Continue");
        }

        /// <summary>
        /// This method is to simulate the email input on sign in page.
        /// </summary>
        /// <param name="email">email.</param>
        public void InputEmail(string email)
        {
            this.emailTextBox.Clear();
            this.emailTextBox.SendKeys(email);
            this.emailTextBox.Click();
        }

        /// <summary>
        /// This method is to simulate the user action clicking on the Continue button.
        /// </summary>
        public void ClickContinueButton()
        {
            this.continueButton.Click();
        }
    }
}
