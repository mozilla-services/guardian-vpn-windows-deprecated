// <copyright file="Settings.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using static FirefoxPrivateNetwork.WireGuard.Config;

namespace FirefoxPrivateNetwork.Migrations
{
    /// <summary>
    /// Used for mirgrating saved settings from config file to settings file.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
        }

        private enum AddressType
        {
            IPv4,
            IPv6,
        }

        /// <summary>
        /// Write IPv4 and IPv6 addresses to settings file from config if they are not already there.
        /// </summary>
        public void MigrateConfigAddressToSettingsFile()
        {
            var filename = ProductConstants.SettingsFile;
            bool overwriteFile = false;

            if (!File.Exists(filename))
            {
                return;
            }

            var parser = new IniParser.FileIniDataParser();
            var data = parser.ReadFile(filename);

            if (data.Sections.ContainsSection("Network"))
            {
                var networkData = data.Sections.GetSectionData("Network");
                string ipAddress;

                if (!networkData.Keys.ContainsKey("IPv4Address") || data["Network"]["IPv4Address"] == string.Empty)
                {
                    ipAddress = GetAddressFromConfig(AddressType.IPv4);

                    if (ipAddress != string.Empty)
                    {
                        data["Network"]["IPv4Address"] = ipAddress;
                        overwriteFile = true;
                    }
                }

                if (!networkData.Keys.ContainsKey("IPv6Address") || data["Network"]["IPv6Address"] == string.Empty)
                {
                    ipAddress = GetAddressFromConfig(AddressType.IPv6);

                    if (ipAddress != string.Empty)
                    {
                        data["Network"]["IPv6Address"] = ipAddress;
                        overwriteFile = true;
                    }
                }

                if (overwriteFile)
                {
                    try
                    {
                        FileInfo file = new FileInfo(filename);
                        Directory.CreateDirectory(file.Directory.FullName);
                        File.WriteAllText(filename, data.ToString());
                    }
                    catch (Exception e)
                    {
                        ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets IP address from config file.
        /// </summary>
        /// <param name="type">IP version address to be returned (IPv4 or IPv6).</param>
        /// <returns>Address of requested IP version.</returns>
        private string GetAddressFromConfig(AddressType type)
        {
            string filePath = ProductConstants.FirefoxPrivateNetworkConfFile;

            if (File.Exists(filePath))
            {
                var parser = new IniParser.FileIniDataParser();
                var iniData = parser.ReadFile(filePath);

                string address = iniData["Interface"]["Address"];
                string[] addresses = address.Split(',');

                IPAddress ipAddress;

                foreach (var addr in addresses)
                {
                    if (addr == string.Empty)
                    {
                        continue;
                    }

                    bool parsedIP = IPAddress.TryParse(addr.Split('/')[0], out ipAddress);

                    if (parsedIP && CheckCorrectAddress(type, ipAddress))
                    {
                        return addr;
                    }
                }
            }

            return string.Empty;
        }

        private bool CheckCorrectAddress(AddressType type, IPAddress address)
        {
            return (type == AddressType.IPv4 && address.AddressFamily == AddressFamily.InterNetwork)
                || (type == AddressType.IPv6 && address.AddressFamily == AddressFamily.InterNetworkV6);
        }
    }
}
