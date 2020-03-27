// <copyright file="Settings.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace FirefoxPrivateNetwork
{
    /// <summary>
    /// Manages the application settings configuration.
    /// </summary>
    internal class Settings
    {
        private FxASettings fxa;
        private LanguageSettings language;
        private NetworkSettings network;
        private string filename;
        private char multiValueSeparator = '|';

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="filename">Filename for file to load settings from.</param>
        public Settings(string filename)
        {
            this.filename = filename;
            var loadedFromFile = LoadSettingsFromFile(filename);
            if (!loadedFromFile)
            {
                fxa = default(FxASettings);
                language = default(LanguageSettings);
                network = default(NetworkSettings);
                network.EnableIPv6 = true;
                ProductConstants.LoadFxAUrls();
            }

            // Configure the VPN allowed IPs
            if (network.EnableIPv6)
            {
                ProductConstants.AllowedIPs = network.AllowLocalDeviceAccess ? ProductConstants.DefaultAllowedIPsLocal : ProductConstants.DefaultAllowedIPs;
            }
            else
            {
                ProductConstants.AllowedIPs = network.AllowLocalDeviceAccess ? ProductConstants.DefaultAllowedIPv4sLocal : ProductConstants.DefaultAllowedIPv4s;
            }
        }

        /// <summary>
        /// Gets or sets FxA settings. Saves settings to file on set.
        /// </summary>
        [DisplayName("FxA")]
        public FxASettings FxA
        {
            get { return fxa; }
            set { SaveSettings(ref fxa, value); }
        }

        /// <summary>
        /// Gets or sets language settings. Saves settings to file on set.
        /// </summary>
        [DisplayName("Language")]
        public LanguageSettings Language
        {
            get { return language; }
            set { SaveSettings(ref language, value); }
        }

        /// <summary>
        /// Gets or sets network settings. Saves settings to file on set.
        /// </summary>
        [DisplayName("Network")]
        public NetworkSettings Network
        {
            get { return network; }
            set { SaveSettings(ref network, value); }
        }

        /// <summary>
        /// Converts settings data to an INI string.
        /// </summary>
        /// <returns>INI string for writing to conf files.</returns>
        public override string ToString()
        {
            var iniData = new IniData();
            InputSettingsConfig(iniData, "FxA", "BaseURL", FxA.BaseURL);
            InputSettingsConfig(iniData, "FxA", "Token", FxA.Token);
            InputSettingsConfig(iniData, "FxA", "PublicKey", FxA.PublicKey);
            InputSettingsConfig(iniData, "Language", "PreferredLanguage", Language.PreferredLanguage);
            InputSettingsConfig(iniData, "Network", "UnsecureNetworkAlert", Network.UnsecureNetworkAlert.ToString());
            InputSettingsConfig(iniData, "Network", "CaptivePortalAlert", Network.CaptivePortalAlert.ToString());
            InputSettingsConfig(iniData, "Network", "CaptivePortalDetectionIp", Network.CaptivePortalDetectionIp.ToString());
            InputSettingsConfig(iniData, "Network", "AllowLocalDeviceAccess", Network.AllowLocalDeviceAccess.ToString());
            InputSettingsConfig(iniData, "Network", "EnableIPv6", Network.EnableIPv6.ToString());
            InputSettingsConfig(iniData, "Network", "IPv4Address", Network.IPv4Address);
            InputSettingsConfig(iniData, "Network", "IPv6Address", Network.IPv6Address);
            InputSettingsConfig(iniData, "Network", "SplitTunnelApps", string.Join(char.ToString(multiValueSeparator), Network.SplitTunnelApps));

            return iniData.ToString();
        }

        private bool ContainsSettingsConfig(IniData data, string section, string key)
        {
            var containsSection = data.Sections.ContainsSection(section);

            if (!containsSection)
            {
                return false;
            }

            var sectionData = data.Sections.GetSectionData(section);

            return sectionData.Keys.ContainsKey(key) && !string.IsNullOrEmpty(sectionData.Keys.GetKeyData(key).Value);
        }

        private bool LoadSettingsFromFile(string settingsFile)
        {
            if (!File.Exists(settingsFile))
            {
                return false;
            }

            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(settingsFile);

                // Loading FxA settings configuration
                LoadFxASettings(data);

                // Loading Language settings configuration
                LoadLanguageSettings(data);

                // Loading Network settings configuration
                LoadNetworkSettings(data);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        private void LoadFxASettings(IniData data)
        {
            var fxaSettings = default(FxASettings);
            if (ContainsSettingsConfig(data, "FxA", "BaseURL"))
            {
                fxaSettings.BaseURL = data["FxA"]["BaseURL"];
                ProductConstants.LoadFxAUrls(fxaSettings.BaseURL);
            }
            else
            {
                ProductConstants.LoadFxAUrls();
            }

            if (ContainsSettingsConfig(data, "FxA", "Token"))
            {
                fxaSettings.Token = data["FxA"]["Token"];
            }

            if (ContainsSettingsConfig(data, "FxA", "PublicKey"))
            {
                fxaSettings.PublicKey = data["FxA"]["PublicKey"];
            }

            fxa = fxaSettings;
        }

        private void LoadLanguageSettings(IniData data)
        {
            var languageSettings = default(LanguageSettings);
            if (ContainsSettingsConfig(data, "Language", "PreferredLanguage"))
            {
                languageSettings.PreferredLanguage = data["Language"]["PreferredLanguage"];
            }

            language = languageSettings;
        }

        private void LoadNetworkSettings(IniData data)
        {
            var networkSettings = default(NetworkSettings);
            if (ContainsSettingsConfig(data, "Network", "UnsecureNetworkAlert"))
            {
                networkSettings.UnsecureNetworkAlert = bool.Parse(data["Network"]["UnsecureNetworkAlert"]);
            }

            if (ContainsSettingsConfig(data, "Network", "CaptivePortalAlert"))
            {
                networkSettings.CaptivePortalAlert = bool.Parse(data["Network"]["CaptivePortalAlert"]);
            }

            if (ContainsSettingsConfig(data, "Network", "CaptivePortalDetectionIp"))
            {
                networkSettings.CaptivePortalDetectionIp = data["Network"]["CaptivePortalDetectionIp"];
            }

            if (ContainsSettingsConfig(data, "Network", "AllowLocalDeviceAccess"))
            {
                networkSettings.AllowLocalDeviceAccess = bool.Parse(data["Network"]["AllowLocalDeviceAccess"]);
            }

            if (ContainsSettingsConfig(data, "Network", "EnableIPv6"))
            {
                networkSettings.EnableIPv6 = bool.Parse(data["Network"]["EnableIPv6"]);
            }
            else
            {
                networkSettings.EnableIPv6 = true;
            }

            if (ContainsSettingsConfig(data, "Network", "IPv4Address"))
            {
                networkSettings.IPv4Address = data["Network"]["IPv4Address"];
            }
            else
            {
                networkSettings.IPv4Address = string.Empty;
            }

            if (ContainsSettingsConfig(data, "Network", "IPv6Address"))
            {
                networkSettings.IPv6Address = data["Network"]["IPv6Address"];
            }
            else
            {
                networkSettings.IPv6Address = string.Empty;
            }

            if (ContainsSettingsConfig(data, "Network", "SplitTunnelApps"))
            {
                networkSettings.SplitTunnelApps = data["Network"]["SplitTunnelApps"].Split(multiValueSeparator);
            }

            network = networkSettings;
        }

        private void SaveSettings<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                WriteSettingsToFile();
            }
        }

        private void InputSettingsConfig(IniData data, string section, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                data[section][key] = value;
            }
        }

        private void WriteSettingsToFile()
        {
            WriteToFile(filename, ToString());
        }

        private bool WriteToFile(string filename, string data)
        {
            try
            {
                FileInfo file = new FileInfo(filename);
                Directory.CreateDirectory(file.Directory.FullName);
                File.WriteAllText(filename, data);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// FxA settings.
        /// </summary>
        public struct FxASettings
        {
            /// <summary>
            /// Gets or sets the base url for the FxA API endpoint.
            /// </summary>
            [DisplayName("BaseURL")]
            public string BaseURL { get; set; }

            /// <summary>
            /// Gets or sets the token for the FxA API endpoint.
            /// </summary>
            [DisplayName("Token")]
            public string Token { get; set; }

            /// <summary>
            /// Gets or sets the public key for the user's VPN device.
            /// </summary>
            [DisplayName("PublicKey")]
            public string PublicKey { get; set; }
        }

        /// <summary>
        /// Language settings.
        /// </summary>
        public struct LanguageSettings
        {
            /// <summary>
            /// Gets or sets the user's preferred language.
            /// </summary>
            [DisplayName("PreferredLanguage")]
            public string PreferredLanguage { get; set; }
        }

        /// <summary>
        /// Network settings.
        /// </summary>
        public struct NetworkSettings
        {
            private bool? unsecureNetworkAlert;
            private bool? captivePortalAlert;

            /// <summary>
            /// Gets or sets a value indicating whether the user's unsecure network alert is on or off.
            /// </summary>
            [DisplayName("UnsecureNetworkAlert")]
            public bool UnsecureNetworkAlert
            {
                get { return unsecureNetworkAlert ?? true; }
                set { unsecureNetworkAlert = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the user's captive portal alert is on or off.
            /// </summary>
            [DisplayName("CaptivePortalAlert")]
            public bool CaptivePortalAlert
            {
                get { return captivePortalAlert ?? true; }
                set { captivePortalAlert = value; }
            }

            /// <summary>
            /// Gets or sets the captive portal detection ip that is resolved from the captive portal detection host.
            /// </summary>
            [DisplayName("CaptivePortalDetectionIp")]
            public string CaptivePortalDetectionIp { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the user's local device access is allowed or not.
            /// </summary>
            [DisplayName("AllowLocalDeviceAccess")]
            public bool AllowLocalDeviceAccess { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the IPv6 is enabled or not.
            /// </summary>
            [DisplayName("EnableIPv6")]
            public bool EnableIPv6 { get; set; }

            /// <summary>
            /// Gets or sets the IPv4 address.
            /// </summary>
            [DisplayName("IPv4Address")]
            public string IPv4Address { get; set; }

            /// <summary>
            /// Gets or sets the IPv6 address.
            /// </summary>
            [DisplayName("IPv6Address")]
            public string IPv6Address { get; set; }

            /// <summary>
            /// Gets or sets the apps that are to be configured for split tunneling.
            /// </summary>
            [DisplayName("SplitTunnelApps")]
            public string[] SplitTunnelApps { get; set; }
        }
    }
}
