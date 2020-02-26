// <copyright file="ExportWindow.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Export Debug Package Window.
    /// </summary>
    internal class ExportWindow
    {
        private AppiumWebElement toolBar;
        private AppiumWebElement fileInput;
        private AppiumWebElement saveButton;
        private AppiumWebElement cancelButton;
        private AppiumWebElement exportWindow;
        private WindowsDriver<WindowsElement> session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportWindow"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="windowName">The window name.</param>
        public ExportWindow(WindowsDriver<WindowsElement> session, string windowName)
        {
            this.session = session;
            this.exportWindow = session.FindElementByName(windowName);
            this.toolBar = this.exportWindow.FindElementByClassName("Breadcrumb Parent");
            var fileInputComboBox = this.exportWindow.FindElementByAccessibilityId("FileNameControlHost");
            this.fileInput = fileInputComboBox.FindElementByName("File name:");
            this.saveButton = this.exportWindow.FindElementByName("Save");
            this.cancelButton = this.exportWindow.FindElementByName("Cancel");
        }

        /// <summary>
        /// Save file on destination.
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="fileName">The file name.</param>
        public void SaveFile(string destination, string fileName)
        {
            // enable the toolbar to be editable
            this.toolBar.FindElementByXPath("//SplitButton").Click();
            var address = this.exportWindow.FindElementByName("Address");
            address.Clear();
            address.SendKeys(destination);
            this.session.Keyboard.SendKeys(Keys.Enter);
            this.fileInput.Clear();
            this.fileInput.SendKeys(fileName);
            this.saveButton.Click();
        }
    }
}