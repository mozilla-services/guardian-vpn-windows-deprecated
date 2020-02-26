// <copyright file="UtilsTest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is to test server selection.
    /// </summary>
    [TestClass]
    public class UtilsTest
    {
        /// <summary>
        /// Test RandomSelectIndex method.
        /// </summary>
        [TestMethod]
        public void TestRandomSelectIndex()
        {
            int randomIndex1 = Utils.RandomSelectIndex(Enumerable.Range(0, 2), (number) => number != 1);
            Assert.AreNotEqual(1, randomIndex1);

            int randomIndex2 = Utils.RandomSelectIndex(Enumerable.Range(0, 2), (number) => number != 0);
            Assert.AreNotEqual(0, randomIndex2);
        }

        [TestMethod]
        public void TestCleanText()
        {
            string text = @"abc 
efg
";
            string cleanText = Utils.CleanText(text);
            Assert.AreEqual("abcefg", cleanText);
        }
    }
}
