// <copyright file="DeviceScreen.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Device screen.
    /// </summary>
    internal class DeviceScreen
    {
        private AppiumWebElement backButton;
        private AppiumWebElement title;
        private AppiumWebElement deviceSummary;
        private AppiumWebElement devicePanelTitle;
        private AppiumWebElement currentDevice;
        private AppiumWebElement currentDeviceName;
        private AppiumWebElement currentDeviceStatus;
        private AppiumWebElement currentDeviceRemoveDeviceButton;
        private AppiumWebElement deviceList;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceScreen"/> class.
        /// </summary>
        /// <param name="vpnSession">VPN session.</param>
        public DeviceScreen(WindowsDriver<WindowsElement> vpnSession)
        {
            var deviceView = Utils.WaitUntilFindElement(vpnSession.FindElementByClassName, "DevicesView");
            this.backButton = deviceView.FindElementByName("Back");
            var textBlocks = deviceView.FindElementsByClassName("TextBlock");
            this.title = textBlocks[0];
            this.deviceSummary = textBlocks[1];
            var devicePanel = deviceView.FindElementByClassName("ScrollViewer");
            this.devicePanelTitle = devicePanel.FindElementByClassName("TextBlock");
            this.deviceList = devicePanel.FindElementByAccessibilityId("DeviceList");
            var deviceListItems = this.deviceList.FindElementsByClassName("ListBoxItem");
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
        /// Get device summary.
        /// </summary>
        /// <returns>The device summary.</returns>
        public string GetDeviceSummary()
        {
            return this.deviceSummary.Text;
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

        /// <summary>
        /// Delete one of other devices.
        /// </summary>
        /// <param name="desktopSession">Desktop session.</param>
        public void RandomDeleteOneDevice(WindowsDriver<WindowsElement> desktopSession)
        {
            var deviceListItems = this.deviceList.FindElementsByClassName("ListBoxItem");
            int originalCount = deviceListItems.Count;
            if (originalCount > 1)
            {
                int randomIndex = Utils.RandomSelectIndex(Enumerable.Range(0, deviceListItems.Count), (i) => i != 0);
                AppiumWebElement randomDevice = deviceListItems[randomIndex];
                AppiumWebElement deleteButton = randomDevice.FindElementByAccessibilityId("DeleteButton");
                deleteButton.Click();

                RemoveDevicePopup removeDevicePopup = new RemoveDevicePopup(desktopSession);
                Assert.AreEqual("Remove device?", removeDevicePopup.GetTitle());
                Assert.IsTrue(removeDevicePopup.GetMessage().StartsWith("Please confirm you would like to remove"));
                removeDevicePopup.ClickRemoveButton();
                int expectedDevices = originalCount - 1;
                int actualDevices = this.GetTotalNumberOfDevices(originalCount - 1);
                Assert.AreEqual(expectedDevices, actualDevices);
            }
        }

        /// <summary>
        /// Get the total number of devices.
        /// </summary>
        /// <returns>The total number of devices.</returns>
        /// <param name="expectedCount">The expected number of devices.</param>
        public int GetTotalNumberOfDevices(int? expectedCount = null)
        {
            ReadOnlyCollection<AppiumWebElement> deviceListItems = null;
            if (expectedCount == null)
            {
                deviceListItems = this.deviceList.FindElementsByClassName("ListBoxItem");
            }
            else
            {
                Utils.WaitUntil(ref deviceListItems, this.deviceList.FindElementsByClassName, "ListBoxItem", (items) => items.Count == expectedCount);
            }

            return deviceListItems.Count;
        }
    }
}
