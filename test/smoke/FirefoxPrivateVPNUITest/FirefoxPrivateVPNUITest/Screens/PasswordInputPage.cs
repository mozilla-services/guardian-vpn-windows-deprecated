// <copyright file="PasswordInputPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for password input page.
    /// </summary>
    internal class PasswordInputPage
    {
        private WindowsElement passwordTextBox;
        private WindowsElement signInButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordInputPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public PasswordInputPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.passwordTextBox = browserSession.FindElementByName("Password");
            this.signInButton = browserSession.FindElementByName("Sign in");
        }

        /// <summary>
        /// This method is to simulate the password input on sign in page.
        /// </summary>
        /// <param name="password">password.</param>
        public void InputPassword(string password)
        {
            this.passwordTextBox.Clear();
            this.passwordTextBox.SendKeys(password);
        }

        /// <summary>
        /// This method is to simulate the user action clicking on the Continue button.
        /// </summary>
        public void ClickSignInButton() => this.signInButton.Click();
    }
}
