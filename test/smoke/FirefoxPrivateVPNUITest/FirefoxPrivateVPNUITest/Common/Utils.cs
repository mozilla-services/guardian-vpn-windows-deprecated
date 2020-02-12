// <copyright file="Utils.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using FirefoxPrivateVPNUITest.Screens;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestSharp;

    /// <summary>
    /// Here are some utilities.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Random select an index from integers based on the condition.
        /// </summary>
        /// <param name="range">A range of integers.</param>
        /// <param name="condition">A condition method.</param>
        /// <returns>A random index.</returns>
        public static int RandomSelectIndex(IEnumerable<int> range, Func<int, bool> condition)
        {
            var result = range.Where(condition);
            var rand = new Random();
            int index = rand.Next(0, result.Count());
            return result.ElementAt(index);
        }
    }
}
