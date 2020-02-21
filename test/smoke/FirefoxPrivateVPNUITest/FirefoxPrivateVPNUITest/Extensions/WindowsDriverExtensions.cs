// <copyright file="WindowsDriverExtensions.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// Here are some extensions for Windows Driver.
    /// </summary>
    public static class WindowsDriverExtensions
    {
        /// <summary>
        /// Wait for {timeOut} milliseconds until find element.
        /// </summary>
        /// <param name="session">Windows driver.</param>
        /// <param name="findMethod">The method used to find element.</param>
        /// <param name="selector">The selector used in the findMethod.</param>
        /// <param name="timeOut">Time out in milliseconds. Default is 10000 milliseconds.</param>
        /// <returns>Windows element.</returns>
        public static WindowsElement WaitUntilFindElement(this WindowsDriver<WindowsElement> session, Func<string, WindowsElement> findMethod, string selector, double timeOut = 10000)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            bool retry = true;
            WindowsElement element = null;
            while (retry && time.ElapsedMilliseconds <= timeOut)
            {
                try
                {
                    element = findMethod(selector);
                    if (element != null)
                    {
                        retry = false;
                        time.Stop();
                    }
                }
                catch (Exception)
                {
                    retry = true;
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }

            if (time.ElapsedMilliseconds > timeOut && element == null)
            {
                time.Stop();
                Assert.Fail($"Unable to find element with {findMethod.Method.Name} - {selector}");
            }

            time.Reset();
            return element;
        }
    }
}
