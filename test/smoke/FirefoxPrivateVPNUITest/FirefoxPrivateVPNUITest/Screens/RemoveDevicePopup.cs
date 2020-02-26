// <copyright file="RemoveDevicePopup.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Remove device popup.
    /// </summary>
    internal class RemoveDevicePopup
    {
        private AppiumWebElement removeButton;
        private AppiumWebElement cancelButton;
        private AppiumWebElement title;
        private AppiumWebElement message;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDevicePopup"/> class.
        /// </summary>
        /// <param name="desktopSession">VPN session.</param>
        public RemoveDevicePopup(WindowsDriver<WindowsElement> desktopSession)
        {
            var removeDevicePopup = desktopSession.FindElementByClassName("Popup");
            this.removeButton = removeDevicePopup.FindElementByName("Remove");
            this.cancelButton = removeDevicePopup.FindElementByName("Ok");
            this.title = removeDevicePopup.FindElementByAccessibilityId("Title");
            this.message = removeDevicePopup.FindElementByAccessibilityId("Message");
        }

        /// <summary>
        /// Get the title on remove device popup.
        /// </summary>
        /// <returns>The title.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Click the Remove button.
        /// </summary>
        public void ClickRemoveButton()
        {
            this.removeButton.Click();
        }

        /// <summary>
        /// Click the cancel button.
        /// </summary>
        public void ClickCancelButton()
        {
            this.cancelButton.Click();
        }

        /// <summary>
        /// Get the message on remove device popup.
        /// </summary>
        /// <returns>The message.</returns>
        public string GetMessage()
        {
            return this.message.Text;
        }
    }
}
