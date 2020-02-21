using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Tests.ServerList
{
    [TestFixture]
    class ServerSelectionTest
    {
        public FxA.ServerList servers;
        private List<Models.CityServerListItem> serverCityList;

        [SetUp]
        public void SetupServerListTest()
        {
            servers = new FxA.ServerList();
            servers.LoadServerDataFromFile(Path.Combine(TestContext.CurrentContext.WorkDirectory, "Fixtures", "servers.json"));
            serverCityList = servers.GetServerCitiesList();
        }

        [Test]
        public void TestSelectServerWhenOneServer()
        {
            var selectedServer = servers.SelectServer(serverCityList.FirstOrDefault(x => x.City == "Brno"));
            Assert.That(selectedServer.Endpoint == "193.9.112.115");
        }

        [Test]
        public void TestSelectServerWhenTwoServers()
        {
            var selectedServer = servers.SelectServer(serverCityList.FirstOrDefault(x => x.City == "Wien"));
            Assert.That(selectedServer.Endpoint == "185.210.219.242" || selectedServer.Endpoint == "5.253.207.34");
        }
    }
}
