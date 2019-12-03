using NUnit.Framework;
using System.Collections.Generic;


namespace FirefoxPrivateNetwork.Tests.ServerList
{
    [TestFixture]
    class RandomPortSelectionTest
    {
        [Test]
        public void TestSelectRandomPort()
        {
            var randomPort = GetRandomPort(100, 200);
            Assert.That(randomPort <= 200 && randomPort >= 100);
        }

        [Test]
        public void TestSelectRandomPortWhenOnlyOne()
        {
            var randomPort = GetRandomPort(53, 53);
            Assert.That(randomPort == 53);
        }

        [Test]
        public void TestSelectRandomPortWhenOnlyTwo()
        {
            var randomPort = GetRandomPort(1, 2);
            Assert.That(randomPort >= 1 && randomPort <= 2);
        }

        [Test]
        public void TestSelectRandomPortWhenMinIsLessThanMax()
        {
            var randomPort = GetRandomPort(50, 10);
            Assert.That(randomPort >= 10 && randomPort <= 50);
        }

        private int GetRandomPort(int minPort, int maxPort)
        {
            var vpn = new FxA.VPNServer();
            vpn.Ports = new List<List<int>> { new List<int> { minPort, maxPort } };

            return vpn.PickRandomPort(vpn.Ports);
        }
    }
}
