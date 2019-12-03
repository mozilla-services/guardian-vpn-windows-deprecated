using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FirefoxPrivateNetwork.Models
{
    /// <summary>
    /// Data binding class that represents a listed server in the combobox of the MainWindow.
    /// </summary>
    public class ServerListItem
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Endpoint { get; set; }
    }

    public class CountryServerListItem
    {
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public List<ServerListItem> Servers { get; set; }

        //public List<CityServerListItem> Cities { get; set; }
    }

    /*
    public class CityServerListItem
    {
        public string Country { get; set; }
        public string City { get; set; }
        public List<ServerListItem> Servers { get; set; }
    }
    */
}
