// <copyright file="NetworkSettingsScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Notification screen.
    /// </summary>
    internal class NetworkSettingsScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement enableIPv6CheckBox;
        private AppiumWebElement enableIPv6Description;
        private AppiumWebElement enableIPv6DisabledMessage;
        private AppiumWebElement allowAccessCheckbox;
        private AppiumWebElement allowAccessDescription;
        private AppiumWebElement allowAccessDisabledMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSettingsScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public NetworkSettingsScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var networkSettingsView = Utils.WaitUntilFindElement(vpnSession.FindElementByClassName, "NetworkSettingsView");
            this.backButton = Utils.WaitUntilFindElement(networkSettingsView.FindElementByName, "Back");
            this.title = Utils.WaitUntilFindElement(networkSettingsView.FindElementByName, "Network settings");
            var networkSettingsPanel = Utils.WaitUntilFindElement(networkSettingsView.FindElementByClassName, "ScrollViewer");
            this.allowAccessCheckbox = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByAccessibilityId, "AllowLocalDeviceAccessCheckBox");
            this.allowAccessDescription = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByName, "Access printers, streaming sticks and all other devices on your local network");
            this.allowAccessDisabledMessage = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByAccessibilityId, "AllowLocalDeviceAccessCheckBoxDisabledMessage");
            this.enableIPv6CheckBox = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByAccessibilityId, "EnableIPv6CheckBox");
            this.enableIPv6Description = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByName, "Push the internet forward with the latest version of the Internet Protocol");
            this.enableIPv6DisabledMessage = Utils.WaitUntilFindElement(networkSettingsPanel.FindElementByAccessibilityId, "EnableIPv6CheckBoxDisabledMessage");
        }

        /// <summary>
        /// Get title on Network settings screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get the text on enable IPv6 checkbox.
        /// </summary>
        /// <returns>The text on enable IPv6 checkbox.</returns>
        public string GetEnableIPv6CheckBoxText()
        {
            return this.enableIPv6CheckBox.Text;
        }

        /// <summary>
        /// Get the description of enable IPv6 checkbox.
        /// </summary>
        /// <returns>The description of enable IPv6 checkbox.</returns>
        public string GetEnableIPv6Description()
        {
            return this.enableIPv6Description.Text;
        }

        /// <summary>
        /// Click the enable IPv6 checkbox.
        /// </summary>
        public void ClickEnableIPv6CheckBox()
        {
            this.enableIPv6CheckBox.Click();
        }

        /// <summary>
        /// Is Enable IPv6 disabled message displayed.
        /// </summary>
        /// <returns>Enable IPv6 disabled message displayed or not.</returns>
        public bool IsEnableIPv6DisabledMessageDisplayed()
        {
            return this.enableIPv6DisabledMessage.Displayed;
        }

        /// <summary>
        /// Is enable IPv6 checked.
        /// </summary>
        /// <returns>Enable IPv6 checked or not.</returns>
        public bool IsEnableIPv6Checked()
        {
            return this.enableIPv6CheckBox.Selected;
        }

        /// <summary>
        /// Is enable IPv6 enabled.
        /// </summary>
        /// <returns>Enable IPv6 enabled or not.</returns>
        public bool IsEnableIPv6Enabled()
        {
            return this.enableIPv6CheckBox.Enabled;
        }

        /// <summary>
        /// Get enable IPv6 disabled message.
        /// </summary>
        /// <returns>Enable IPv6 disabled message</returns>
        public string GetEnableIPv6DisabledMessage()
        {
            return this.enableIPv6DisabledMessage.Text;
        }

        /// <summary>
        /// Get the text on Allow access checkbox.
        /// </summary>
        /// <returns>The text on allow access checkbox.</returns>
        public string GetAllowAccessText()
        {
            return this.allowAccessCheckbox.Text;
        }

        /// <summary>
        /// Get the description of the allow access checkbox.
        /// </summary>
        /// <returns>The description of the allow access checkbox.</returns>
        public string GetAllowAccessDescription()
        {
            return this.allowAccessDescription.Text;
        }

        /// <summary>
        /// Click the Allow Access check box.
        /// </summary>
        public void ClickAllowAccessCheckBox()
        {
            this.allowAccessCheckbox.Click();
        }

        /// <summary>
        /// Is Allow access disabled message displayed.
        /// </summary>
        /// <returns>Allow access disabled message displayed or not.</returns>
        public bool IsAllowAccessDisabledMessageDisplayed()
        {
            return this.allowAccessDisabledMessage.Displayed;
        }

        /// <summary>
        /// Is Allow access checked.
        /// </summary>
        /// <returns>Allow access checked or not.</returns>
        public bool IsAllowAccessChecked()
        {
            return this.allowAccessCheckbox.Selected;
        }

        /// <summary>
        /// Is allow access enabled.
        /// </summary>
        /// <returns>Allow access enabled or not.</returns>
        public bool IsAllowAccessEnabled()
        {
            return this.allowAccessCheckbox.Enabled;
        }

        /// <summary>
        /// Get allow access disabled message.
        /// </summary>
        /// <returns>Allow access disabled message</returns>
        public string GetAllowAccessDisabledMessage()
        {
            return this.allowAccessDisabledMessage.Text;
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }
    }
}
