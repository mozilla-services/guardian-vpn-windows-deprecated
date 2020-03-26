using System;
using NUnit.Framework;

namespace FirefoxPrivateNetwork.Tests.Versioning
{
    [TestFixture]
    class VersioningTest
    {
        [Test]
        public void TestBasicVersionStringParsingWorks()
        {
            FxA.Version v;

            v = new FxA.Version("0.8a");
            Assert.AreEqual("0.8a", v.ToString());

            v = new FxA.Version("0.4b");
            Assert.AreEqual("0.4b", v.ToString());

            v = new FxA.Version("0.7");
            Assert.AreEqual("0.7", v.ToString());
        }

        [Test]
        public void TestAdvancedVersionStringParsingWorks()
        {
            FxA.Version v;

            v = new FxA.Version("0.82a");
            Assert.AreEqual("0.82a", v.ToString());

            v = new FxA.Version("4.20b");
            Assert.AreEqual("4.20b", v.ToString());

            v = new FxA.Version("3.5,z");
            Assert.AreEqual("3.5a", v.ToString());

            v = new FxA.Version("4.1 Wookie");
            Assert.AreEqual("4.1a", v.ToString());
        }

        [Test]
        public void TestBadVersionStringParsing()
        {
            FxA.Version v;

            Assert.Throws<FormatException>(() =>
            {
                v = new FxA.Version("0.5.1a");
            });

            Assert.Throws<FormatException>(() =>
            {
                v = new FxA.Version("0....1");
            });

            Assert.Throws<FormatException>(() =>
            {
                v = new FxA.Version("hello");
            });

            Assert.Throws<FormatException>(() =>
            {
                v = new FxA.Version("h4x0r");
            });
        }

        [Test]
        public void TestVersionComparison()
        {
            FxA.Version v1, v2, v3;

            v1 = new FxA.Version("0.8a");
            v2 = new FxA.Version("0.7a");
            v3 = new FxA.Version("0.8a");

            Assert.That(v1.CompareTo(v2) == 1);
            Assert.That(v2.CompareTo(v1) == -1);
            Assert.That(v1.CompareTo(v3) == 0);
        }

        [Test]
        public void TestVersionComparisonWithDifferentRelease()
        {
            // Release types are ignored for now
            FxA.Version v1, v2;

            v1 = new FxA.Version("0.8a");
            v2 = new FxA.Version("0.8b");

            Assert.That(v1.CompareTo(v2) == 0);
        }
    }
}
