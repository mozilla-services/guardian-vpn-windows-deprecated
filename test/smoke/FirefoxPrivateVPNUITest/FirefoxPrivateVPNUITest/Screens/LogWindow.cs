// <copyright file="LogWindow.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest.Screens
{
    using System.Collections.Generic;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    /// <summary>
    /// This model is for Log Window.
    /// </summary>
    internal class LogWindow
    {
        private AppiumWebElement datagrid;
        private AppiumWebElement saveButton;
        private AppiumWebElement timeStampColumn;
        private AppiumWebElement messageColumn;
        private AppiumWebElement logWindow;
        private IReadOnlyCollection<AppiumWebElement> logRows;
        private WindowsDriver<WindowsElement> desktopSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogWindow"/> class.
        /// </summary>
        /// <param name="desktopSession">Desktop session.</param>
        public LogWindow(WindowsDriver<WindowsElement> desktopSession)
        {
            this.desktopSession = desktopSession;
            this.logWindow = desktopSession.FindElementByName("Log");
            this.datagrid = this.logWindow.FindElementByAccessibilityId("logView");
            this.saveButton = this.logWindow.FindElementByName("Save");
            var datagridColumns = this.datagrid.FindElementsByClassName("GridViewColumnHeader");
            this.timeStampColumn = datagridColumns[0];
            this.messageColumn = datagridColumns[1];
            this.logRows = this.datagrid.FindElementsByClassName("ListViewItem");
        }

        /// <summary>
        /// Get Timestamp column header.
        /// </summary>
        /// <returns>The header text on timestamp column.</returns>
        public string GetTimeStampColumnHeader()
        {
            return this.timeStampColumn.Text;
        }

        /// <summary>
        /// Get message column header.
        /// </summary>
        /// <returns>The header text on message column.</returns>
        public string GetMessageColumnHeader()
        {
            return this.messageColumn.Text;
        }

        /// <summary>
        /// Click Save button.
        /// </summary>
        public void ClickSaveButton()
        {
            this.saveButton.Click();
        }

        /// <summary>
        /// Get the number of logs listed in grid.
        /// </summary>
        /// <returns>The number of logs.</returns>
        public int GetNumberOfLogs()
        {
            return this.logRows.Count;
        }

        /// <summary>
        /// Close Log window.
        /// </summary>
        public void CloseWindow()
        {
            var titleBar = this.logWindow.FindElementByXPath("//TitleBar");
            this.desktopSession.Mouse.ContextClick(titleBar.Coordinates);
            var closeItem = this.desktopSession.FindElementByName("Close");
            closeItem.Click();
        }
    }
}