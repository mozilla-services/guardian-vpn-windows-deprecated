using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FirefoxPrivateNetwork.Tests.ServerList
{
    [TestFixture]
    class SortingAndRetrievalTest
    {
        private FxA.ServerList servers;
        private List<Models.CityServerListItem> serverCityList;

        [SetUp]
        public void SetupServerListTest()
        {
            servers = new FxA.ServerList();
            servers.LoadServerDataFromFile(Path.Combine(TestContext.CurrentContext.WorkDirectory, "Fixtures", "servers.json"));
            serverCityList = servers.GetServerCitiesList();
        }

        [Test]
        public void TestLoadServerDataFromFile()
        {
            // For test coverage purposes, we need to call this again
            servers.LoadServerDataFromFile(Path.Combine(TestContext.CurrentContext.WorkDirectory, "Fixtures", "servers.json"));

            Assert.AreEqual(5, serverCityList.Count);

            var vpnServerItems = servers.GetServerItems();
            Assert.AreEqual(9, vpnServerItems.Count);

            // Verify if all countries are present
            Assert.AreEqual(4, vpnServerItems.Where(i => i.Value.Country == "Czech Republic").Count());
            Assert.AreEqual(1, vpnServerItems.Where(i => i.Value.Country == "Hong Kong").Count());
            Assert.AreEqual(2, vpnServerItems.Where(i => i.Value.Country == "Serbia").Count());
            Assert.AreEqual(2, vpnServerItems.Where(i => i.Value.Country == "Austria").Count());

            // Verify cities
            Assert.AreEqual(2, vpnServerItems.Where(i => i.Value.City == "Wien").Count());
            Assert.AreEqual(1, vpnServerItems.Where(i => i.Value.City == "Hong Kong").Count());
            Assert.AreEqual(2, vpnServerItems.Where(i => i.Value.City == "Belgrade").Count());
            Assert.AreEqual(3, vpnServerItems.Where(i => i.Value.City == "Prague").Count());
            Assert.AreEqual(1, vpnServerItems.Where(i => i.Value.City == "Brno").Count());

            // Verify data for Wien
            var verifyServerItem = vpnServerItems.Where(i => i.Value.City == "Hong Kong").First();

            Assert.AreEqual("10.64.0.1", verifyServerItem.Value.DNSServerAddress);
            Assert.AreEqual("209.58.188.180", verifyServerItem.Value.Endpoint);
            Assert.AreEqual("10.64.0.1", verifyServerItem.Value.IPv4Address);
            Assert.AreEqual("fc00:bbbb:bbbb:bb01::1", verifyServerItem.Value.IPv6Address);
            Assert.AreEqual("ZlAoBnq2CCqVfyHVkohdXRGGRdEXax65TdsS+CjjKmA=", verifyServerItem.Value.PublicKey);
            Assert.AreEqual(100, verifyServerItem.Value.Weight);
            Assert.AreEqual("hk1-wireguard", verifyServerItem.Value.Name);

            // Verify ports for Wien
            Assert.AreEqual(3, verifyServerItem.Value.Ports.Count);

            Assert.AreEqual(53, verifyServerItem.Value.Ports[0][0]);
            Assert.AreEqual(53, verifyServerItem.Value.Ports[0][1]);

            Assert.AreEqual(10000, verifyServerItem.Value.Ports[1][0]);
            Assert.AreEqual(51820, verifyServerItem.Value.Ports[1][1]);

            Assert.AreEqual(52000, verifyServerItem.Value.Ports[2][0]);
            Assert.AreEqual(60000, verifyServerItem.Value.Ports[2][1]);
        }

        [Test]
        public void TestGetServerByIndex()
        {
            var testServer = servers.GetServerByIndex(0);
            Assert.AreEqual("Austria", testServer.Country);
            Assert.AreEqual("Wien", testServer.City);
            Assert.AreEqual("at1-wireguard", testServer.Name);
        }

        [Test]
        public void TestGetServerCityList()
        {
            var testServerCities = servers.GetServerCitiesList();
            Assert.AreEqual(5, testServerCities.Count);

            var testCity = testServerCities[0];
            Assert.AreEqual("Wien", testCity.City);
        }

        [Test]
        public void TestGetServerSelection()
        {
            var testServerCities = servers.GetServerCitiesList();
            Assert.AreEqual(5, testServerCities.Count);

            var testServer = testServerCities[0].Servers[0];
            Assert.AreEqual("Wien 1", testServer.Name);
        }

        [Test]
        public void TestGetServerListByIndex()
        {
            var testServerIP = servers.GetServerIPByIndex(0);
            Assert.That(testServerIP.StartsWith("185.210.219.242"));
        }

        [Test]
        public void TestGetServerPublicKeyByIndex()
        {
            var testServerKey = servers.GetServerPublicKeyByIndex(0);
            Assert.AreEqual("iE7SukqspT1UtQxce9S5plJ+GpAXdl4zG2oqpbhzvAw=", testServerKey);
        }

        [Test]
        public void TestGetServerIndexByIP()
        {
            var testServerIndex = servers.GetServerIndexByIP("185.210.219.242");
            Assert.AreEqual(0, testServerIndex);
        }

        [Test]
        public void TestSortServerList()
        {
            var shuffledServerList = servers.GetServerItems();

            shuffledServerList[1].Country = "Aardvarkia, Republic Of";
            shuffledServerList[0].Country = "Zeezeekia, Kingdom Of";

            var testServerList = servers.SortServerList(shuffledServerList);

            Assert.AreEqual(testServerList.First().Value.Name, shuffledServerList[1].Name);
            Assert.AreEqual(testServerList.Last().Value.Name, shuffledServerList[0].Name);
        }
    }
}
