// <copyright file="ManageAccountPage.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for account manament page.
    /// </summary>
    internal class ManageAccountPage
    {
        private WindowsDriver<WindowsElement> browserSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageAccountPage"/> class.
        /// </summary>
        /// <param name="browserSession">browser session.</param>
        public ManageAccountPage(WindowsDriver<WindowsElement> browserSession)
        {
            this.browserSession = browserSession;
        }

        /// <summary>
        /// Click delete button.
        /// </summary>
        public void ClickDeleteButton()
        {
            this.browserSession.Keyboard.SendKeys(Keys.PageDown);
            WindowsElement deleteButton = null;
            Utils.WaitUntil(ref deleteButton, this.browserSession.FindElementByName, "Delete…", (result) => result != null && result.Displayed);
            deleteButton.Click();
        }

        /// <summary>
        /// To confirm deleting the account.
        /// </summary>
        /// <param name="password">The new user password.</param>
        public void ConfirmDeleteAccount(string password)
        {
            this.browserSession.Keyboard.SendKeys(Keys.PageDown);
            Utils.WaitUntilFindElement(this.browserSession.FindElementByAccessibilityId, "delete-account-subscriptions").Click();
            Utils.WaitUntilFindElement(this.browserSession.FindElementByAccessibilityId, "delete-account-saved-info").Click();
            Utils.WaitUntilFindElement(this.browserSession.FindElementByAccessibilityId, "delete-account-reactivate").Click();

            var passwordInput = Utils.WaitUntilFindElement(this.browserSession.FindElementByName, "Password");
            passwordInput.SendKeys(password);

            Utils.WaitUntilFindElement(this.browserSession.FindElementByName, "Delete account").Click();
        }
    }
}
