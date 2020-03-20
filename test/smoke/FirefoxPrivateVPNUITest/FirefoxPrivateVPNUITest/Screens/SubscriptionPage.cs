// <copyright file="SubscriptionPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for verification code page.
    /// </summary>
    internal class SubscriptionPage
    {
        private AppiumWebElement fullNameInput;
        private AppiumWebElement cardNumberInput;
        private AppiumWebElement expDateInput;
        private AppiumWebElement cvcInput;
        private AppiumWebElement zipCodeInput;
        private AppiumWebElement authorizeCheckbox;
        private AppiumWebElement submit;
        private WindowsDriver<WindowsElement> browserSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public SubscriptionPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.browserSession = browserSession;
            this.fullNameInput = Utils.WaitUntilFindElement(browserSession.FindElementByName, "Name as it appears on your card Full Name");
            this.cardNumberInput = Utils.WaitUntilFindElement(browserSession.FindElementByName, "Credit or debit card number");
            this.expDateInput = Utils.WaitUntilFindElement(browserSession.FindElementByName, "Credit or debit card expiration date");
            this.cvcInput = Utils.WaitUntilFindElement(browserSession.FindElementByName, "Credit or debit card CVC/CVV");
            this.zipCodeInput = Utils.WaitUntilFindElement(browserSession.FindElementByName, "ZIP code 12345");
            this.authorizeCheckbox = Utils.WaitUntilFindElement(browserSession.FindElementByName, "I authorize Mozilla, maker of Firefox products, to charge my payment method $9.99 per month , according to payment terms, until I cancel my subscription.");
            this.submit = Utils.WaitUntilFindElement(browserSession.FindElementByName, "Submit");
        }

        /// <summary>
        /// Input full name.
        /// </summary>
        /// <param name="fullName">Full name.</param>
        public void InputFullName(string fullName)
        {
            this.fullNameInput.Click();
            this.fullNameInput.Clear();
            this.fullNameInput.SendKeys(fullName);
        }

        /// <summary>
        /// Input card number.
        /// </summary>
        /// <param name="cardNumber">Card number.</param>
        public void InputCardNumber(string cardNumber)
        {
            this.cardNumberInput.Click();
            this.cardNumberInput.Clear();
            this.cardNumberInput.SendKeys(cardNumber);
        }

        /// <summary>
        /// Input exp date.
        /// </summary>
        /// <param name="expDate">Exp. date.</param>
        public void InputExpDate(string expDate)
        {
            this.expDateInput.Click();
            this.expDateInput.Clear();
            this.expDateInput.SendKeys(expDate);
        }

        /// <summary>
        /// Input cvc.
        /// </summary>
        /// <param name="cvc">CVC.</param>
        public void InputCVC(string cvc)
        {
            this.cvcInput.Click();
            this.cvcInput.Clear();
            this.cvcInput.SendKeys(cvc);
        }

        /// <summary>
        /// Input zip code.
        /// </summary>
        /// <param name="zipCode">Zip code.</param>
        public void InputZipCode(string zipCode)
        {
            this.zipCodeInput.Click();
            this.zipCodeInput.Clear();
            this.zipCodeInput.SendKeys(zipCode);
        }

        /// <summary>
        /// Check the authorize checkbox.
        /// </summary>
        public void ClickAuthorizeCheckBox()
        {
            this.authorizeCheckbox.Click();
        }

        /// <summary>
        /// Click submit button.
        /// </summary>
        public void ClickSubmitButton()
        {
            try
            {
                this.submit.Click();
            }
            catch (Exception ex)
            {
                if (ex.Message == "An element command could not be completed because the element is not pointer- or keyboard interactable.")
                {
                    Utils.WaitUntilFindElement(this.browserSession.FindElementByName, "Submit").Click();
                }
            }
        }
    }
}
