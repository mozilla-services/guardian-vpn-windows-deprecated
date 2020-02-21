// <copyright file="DeviceScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System;
    using System.Threading;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Device screen.
    /// </summary>
    internal class DeviceScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement numberOfDevices;
        private AppiumWebElement devicePanelTitle;
        private AppiumWebElement currentDevice;
        private AppiumWebElement currentDeviceName;
        private AppiumWebElement currentDeviceStatus;
        private AppiumWebElement currentDeviceRemoveDeviceButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public DeviceScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var deviceView = vpnSession.FindElementByClassName("DevicesView");
            this.backButton = deviceView.FindElementByName("Back");
            var textBlocks = deviceView.FindElementsByClassName("TextBlock");
            this.title = textBlocks[0];
            this.numberOfDevices = textBlocks[1];
            var devicePanel = deviceView.FindElementByClassName("ScrollViewer");
            this.devicePanelTitle = devicePanel.FindElementByClassName("TextBlock");
            var deviceList = devicePanel.FindElementByAccessibilityId("DeviceList");
            var deviceListItems = deviceList.FindElementsByClassName("ListBoxItem");
            this.currentDevice = deviceListItems[0];
            var currentDeviceTextBlocks = this.currentDevice.FindElementsByClassName("TextBlock");
            this.currentDeviceName = currentDeviceTextBlocks[0];
            this.currentDeviceStatus = currentDeviceTextBlocks[1];
            this.currentDeviceRemoveDeviceButton = this.currentDevice.FindElementByAccessibilityId("DeleteButton");
        }

        /// <summary>
        /// Get title on Device screen.
        /// </summary>
        /// <returns>The title string.</returns>
        public string GetTitle()
        {
            return this.title.Text;
        }

        /// <summary>
        /// Get number of device.
        /// </summary>
        /// <returns>The number of device.</returns>
        public string GetNumberOfDevice()
        {
            return this.numberOfDevices.Text;
        }

        /// <summary>
        /// Get the device panel title.
        /// </summary>
        /// <returns>The device panel title.</returns>
        public string GetDevicePanelTitle()
        {
            return this.devicePanelTitle.Text;
        }

        /// <summary>
        /// Get current device name.
        /// </summary>
        /// <returns>The current device name.</returns>
        public string GetCurrentDeviceName()
        {
            return this.currentDeviceName.Text;
        }

        /// <summary>
        /// Get current device status.
        /// </summary>
        /// <returns>The current device status.</returns>
        public string GetCurrentDeviceStatus()
        {
            return this.currentDeviceStatus.Text;
        }

        /// <summary>
        /// Get the current device remove button.
        /// </summary>
        /// <returns>Current device remove button.</returns>
        public AppiumWebElement GetCurrentDeviceRemoveButton()
        {
            return this.currentDeviceRemoveDeviceButton;
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
