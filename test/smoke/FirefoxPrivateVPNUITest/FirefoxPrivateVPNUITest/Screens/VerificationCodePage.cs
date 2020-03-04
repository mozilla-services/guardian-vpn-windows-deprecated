// <copyright file="VerificationCodePage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for verification code page.
    /// </summary>
    internal class VerificationCodePage
    {
        private WindowsElement verificationCodeInput;
        private WindowsElement verifyButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationCodePage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public VerificationCodePage(WindowsDriver<WindowsElement> browserSession)
        {
            this.verificationCodeInput = browserSession.FindElementByName("Enter 6-digit code");
            this.verifyButton = browserSession.FindElementByName("Verify");
        }

        /// <summary>
        /// Input the verification code.
        /// </summary>
        /// <param name="verificationCode">Verification code.</param>
        public void InputVerificationCode(string verificationCode)
        {
            this.verificationCodeInput.Click();
            this.verificationCodeInput.Clear();
            this.verificationCodeInput.SendKeys(verificationCode);
        }

        /// <summary>
        /// Click verify button.
        /// </summary>
        public void ClickVerifyButton()
        {
            this.verifyButton.Click();
        }
    }
}
