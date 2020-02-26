// <copyright file="NotificationsScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Notification screen.
    /// </summary>
    internal class NotificationsScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement unsecuredNetworkAlertCheckBox;
        private AppiumWebElement unsecuredNetworkAlertDescription;
        private AppiumWebElement guestWifiPortalAlertCheckBox;
        private AppiumWebElement guestWifiPortalAlertDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public NotificationsScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var notificationsView = vpnSession.FindElementByClassName("NotificationsView");
            this.backButton = notificationsView.FindElementByName("Back");
            this.title = notificationsView.FindElementByName("Notifications");
            var notificationsPane = notificationsView.FindElementByClassName("ScrollViewer");
            this.unsecuredNetworkAlertCheckBox = notificationsPane.FindElementByName("Unsecured network alert");
            this.guestWifiPortalAlertCheckBox = notificationsPane.FindElementByName("Guest Wi-Fi portal alert");
            var textBlocks = notificationsPane.FindElementsByClassName("TextBlock");
            this.unsecuredNetworkAlertDescription = textBlocks[0];
            this.guestWifiPortalAlertDescription = textBlocks[1];
        }

        /// <summary>
        /// Get title on Notifications screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get the text on unsecured network alert checkbox.
        /// </summary>
        /// <returns>The text on unsecured network alert checkbox.</returns>
        public string GetUnsecuredNetworkAlertText()
        {
            return this.unsecuredNetworkAlertCheckBox.Text;
        }

        /// <summary>
        /// Get the text on guest wifi portal alert checkbox.
        /// </summary>
        /// <returns>The text on guest wifi portal alert checkbox.</returns>
        public string GetGuestWifiPortalAlertText()
        {
            return this.guestWifiPortalAlertCheckBox.Text;
        }

        /// <summary>
        /// Get the description of the unsecured network alert.
        /// </summary>
        /// <returns>The description of the unsecured network alert.</returns>
        public string GetUnsecuredNetworkAlertDescription()
        {
            return this.unsecuredNetworkAlertDescription.Text;
        }

        /// <summary>
        /// Get the description of the unsecured network alert.
        /// </summary>
        /// <returns>The description of the unsecured network alert.</returns>
        public string GetGuestWifiPortalAlertDescription()
        {
            return this.guestWifiPortalAlertDescription.Text;
        }

        /// <summary>
        /// Click the unsecured network alert checkbox.
        /// </summary>
        public void ClickUnsecuredNetworkAlertCheckBox()
        {
            this.unsecuredNetworkAlertCheckBox.Click();
        }

        /// <summary>
        /// Click the guest wifi portal alert checkbox.
        /// </summary>
        public void ClickGuestWifiPortalAlertCheckBox()
        {
            this.guestWifiPortalAlertCheckBox.Click();
        }

        /// <summary>
        /// Click the Back button.
        /// </summary>
        public void ClickBackButton()
        {
            this.backButton.Click();
        }

        /// <summary>
        /// Is unsecured network alert checked.
        /// </summary>
        /// <returns>Unsecured network alert checked or not.</returns>
        public bool IsUnsecuredNetworkAlertChecked()
        {
            return this.unsecuredNetworkAlertCheckBox.Selected;
        }

        /// <summary>
        /// Is guest wifi portal alert checked.
        /// </summary>
        /// <returns>Guest wifi portal checked or not.</returns>
        public bool IsGuestWifiPortalAlertChecked()
        {
            return this.guestWifiPortalAlertCheckBox.Selected;
        }
    }
}
