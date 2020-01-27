// <copyright file="RegisterPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for registration page.
    /// </summary>
    internal class RegisterPage
    {
        private WindowsElement passwordTextBox;
        private WindowsElement repeatPasswordTextBox;
        private WindowsElement createAccountButton;
        private WindowsElement ageTextBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public RegisterPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.passwordTextBox = browserSession.FindElementByName("Password");
            this.repeatPasswordTextBox = browserSession.FindElementByName("Repeat password");
            this.createAccountButton = browserSession.FindElementByName("Create account");
            this.ageTextBox = browserSession.FindElementByName("How old are you?");
        }

        /// <summary>
        /// Input the password on registration page.
        /// </summary>
        /// <param name="password">user password.</param>
        public void InputPassword(string password)
        {
            this.passwordTextBox.Clear();
            this.passwordTextBox.SendKeys(password);
        }

        /// <summary>
        /// Input the password again on registration page.
        /// </summary>
        /// <param name="repeatPassword">user password.</param>
        public void InputRepeatPassword(string repeatPassword)
        {
            this.repeatPasswordTextBox.Clear();
            this.repeatPasswordTextBox.SendKeys(repeatPassword);
        }

        /// <summary>
        /// Click Create Account button.
        /// </summary>
        public void ClickCreateAccountButton()
        {
            this.createAccountButton.Click();
        }

        /// <summary>
        /// Input age.
        /// </summary>
        /// <param name="age">user age.</param>
        public void InputAge(string age)
        {
            this.ageTextBox.Clear();
            this.ageTextBox.SendKeys(age);
        }
    }
}
