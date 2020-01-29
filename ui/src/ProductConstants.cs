// <copyright file="ProductConstants.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using IniParser;
using IniParser.Model;

namespace FirefoxPrivateNetwork
{
    /// <summary>
    /// Product constants used app-wide.
    /// </summary>
    public class ProductConstants
    {
        /// <summary>
        /// Main product name.
        /// </summary>
        public const string ProductName = "Firefox Private Network VPN";

        /// <summary>
        /// Notification area/system tray tool tip.
        /// </summary>
        public const string DefaultSystemTrayTitle = "Firefox Private Network VPN";

        /// <summary>
        /// Tray windows class name used when constructing the TrayWindowMessageHandler. Must be unique.
        /// </summary>
        public const string TrayWindowClassName = InternalAppName + "TrayWindowMessageHandler";

        /// <summary>
        /// Tunnel service internal application name.
        /// </summary>
        public const string InternalAppName = "FirefoxPrivateNetworkVPN";

        /// <summary>
        /// Name of the broker service.
        /// </summary>
        public const string BrokerServiceName = "FirefoxPrivateNetworkVPNBroker";

        /// <summary>
        /// Tunnel service unique name. Must be prefixed with "WireGuard$".
        /// </summary>
        public const string TunnelServiceInternalName = "WireGuardTunnel$FirefoxPrivateNetworkVPN";

        /// <summary>
        /// Tunnel service display name.
        /// </summary>
        public const string TunnelServiceName = "Firefox Private Network VPN";

        /// <summary>
        /// Tunnel service description.
        /// </summary>
        public const string TunnelServiceDescription = "Manages the Firefox Private Network VPN tunnel connection";

        /// <summary>
        /// Tunnel service pipe name. Must be unique and prefixed with "ProtectedPrefix\Administrators\WireGuard\".
        /// </summary>
        public const string TunnelPipeName = "ProtectedPrefix\\Administrators\\WireGuard\\FirefoxPrivateNetworkVPN";

        /// <summary>
        /// Default filename suggestion when saving a log file dump.
        /// </summary>
        public const string DefaultViewLogSaveFilename = "fpn-log.txt";

        /// <summary>
        /// Template URL for Balrog/updating.
        /// {0} - Current application version (e.g. 0.5.0).
        /// {1} - Current platform (e.g. WINNT_x86_64).
        /// </summary>
#if DEBUG_QA
        public const string UpdateTemplateUrl = "http://127.0.0.1:8080/json/1/FirefoxVPN/{0}/{1}/release/update.json";
#else
        public const string UpdateTemplateUrl = "https://aus5.mozilla.org/json/1/FirefoxVPN/{0}/{1}/release/update.json";
#endif

        /// <summary>
        /// Used for preventing two instances of the app from running at the same time.
        /// </summary>
        public static readonly string GUID = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value;

        /// <summary>
        /// User's app data folder where the configuration files are stored.
        /// </summary>
        public static readonly string UserAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mozilla", "FirefoxPrivateNetworkVPN");

        /// <summary>
        /// VPN WG configuration file path.
        /// </summary>
        public static readonly string FirefoxPrivateNetworkConfFile = Path.Combine(UserAppDataFolder, "FirefoxPrivateNetworkVPN.conf");

        /// <summary>
        /// Log file location.
        /// </summary>
        public static readonly string FirefoxPrivateNetworkLogFile = Path.Combine(UserAppDataFolder, "log.bin");

        /// <summary>
        /// FxA user file, containing FxA user data received at login.
        /// </summary>
        public static readonly string FxAUserFile = Path.Combine(UserAppDataFolder, "FirefoxFxAUser.json");

        /// <summary>
        /// Settings file, containing user's settings configuration.
        /// </summary>
        public static readonly string SettingsFile = Path.Combine(UserAppDataFolder, "settings.conf");

        /// <summary>
        /// Default UTM tags to append to FxA URLs when making requests.
        /// </summary>
        public static readonly string FxAUtmTags = "?utm_medium=fx-vpn&utm_source=fx-vpn-windows&utm_campaign=download-client";

        /// <summary>
        /// Catch all IPv4 and IPv6 addresses.
        /// </summary>
        public static readonly string DefaultAllowedIPs = "0.0.0.0/0,::0/0";

        /// <summary>
        /// 0.0.0.0/0 (IPv4) and ::0/0 (IPv6) separated into two equally sized subnets for routing, while allowing local network subnets to take precedent in the routing table.
        /// </summary>
        public static readonly string DefaultAllowedIPsLocal = "0.0.0.0/1, 128.0.0.0/1, ::/1, 8000::/1";

        /// <summary>
        /// Number of seconds without a valid keepalive packet within the tunnel, after which the connection is deemed to be in a "no signal" state.
        /// </summary>
        public static readonly int TunnelNoSignalTimeout = 200;

        /// <summary>
        /// Number of seconds without any received data within the tunnel, after which the connection is demmed to be unstable.
        /// </summary>
        public static readonly int TunnelUnstableTimeout = 10;

        /// <summary>
        /// Number of seconds to wait after connection before acknowledging any connection trouble.
        /// </summary>
        public static readonly int TunnelInitialGracePeriodTimeout = 10;

        /// <summary>
        /// Number of seconds after which failed communication efforts with the Broker will be marked as troublesome and acted upon.
        /// </summary>
        public static readonly int BrokerToubleGracePeriod = 5;

        /// <summary>
        /// Number of seconds to wait before displaying another "insecure network detected" toast popup upon connecting to an unsecured WiFi network.
        /// </summary>
        public static readonly int UnsecureWiFiTimeout = 20;

        /// <summary>
        /// IP address housing the captive portal detection TXT file.
        /// </summary>
        public static readonly string CaptivePortalDetectionIP = "184.150.160.34";

        /// <summary>
        /// Hostname to use when contacting CaptivePortalDetectionIP for captive portal detection.
        /// </summary>
        public static readonly string CaptivePortalDetectionHost = "detectportal.firefox.com";

        /// <summary>
        /// Full URL to attempt to download during the captive portal detection process. %s will be replaced with CaptivePortalDetectionHost.
        /// </summary>
        public static readonly string CaptivePortalDetectionUrl = "http://%s/success.txt";

        /// <summary>
        /// Contents of the TXT file downloaded when checking against a captive portal being active.
        /// </summary>
        public static readonly string CaptivePortalDetectionValidReplyContents = "success";

        /// <summary>
        /// Gets or sets a value indicating whether developer mode is on. Developer mode is on when the default FxA base URL has been overridden.
        /// </summary>
        public static bool IsDevMode { get; set; } = false;

        /// <summary>
        /// Gets or sets the currently used base URL for communicating with FxA.
        /// </summary>
#if DEBUG_QA
        public static string BaseUrl = "http://127.0.0.1:8080";
#else
        public static string BaseUrl { get; set; } = "https://fpn.firefox.com";
#endif

        /// <summary>
        /// Gets or sets the list of addresses that the VPN will be able to send traffic to.
        /// </summary>
        public static string AllowedIPs { get; set; }

        /// <summary>
        /// Gets or sets the constructed FxA API URL.
        /// </summary>
        public static string FxAUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed FxA user login URL.
        /// </summary>
        public static string FxALoginUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed feedback form URL.
        /// </summary>
        public static string FeedbackFormUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed subscription URL.
        /// </summary>
        public static string SubscriptionUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed VPN client download URL.
        /// </summary>
        public static string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed VPN FxA management URL.
        /// </summary>
        public static string FxAAccountManagementUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed support URL.
        /// </summary>
        public static string SupportUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed contact URL.
        /// </summary>
        public static string ContactUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed terms and conditions URL.
        /// </summary>
        public static string TermsUrl { get; set; }

        /// <summary>
        /// Gets or sets the constructed privacy policy URL.
        /// </summary>
        public static string PrivacyUrl { get; set; }

        /// <summary>
        /// Gets or sets the logging level for the whole application.
        /// </summary>
        public static ErrorHandling.LogLevel LogLevel { get; set; }

        /// <summary>
        /// Builds a user agent string based on the current system details.
        /// </summary>
        /// <returns>User agent string.</returns>
        public static string GetUserAgent()
        {
            var osVersion = Environment.OSVersion.ToString();
            var osBits = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            return string.Format("{0}/{1} ({2}; {3})", ProductName, GetVersion(), osVersion, osBits);
        }

        /// <summary>
        /// Retrieves the FPN VPN style version number (such as 0.40b).
        /// </summary>
        /// <returns>Version number with optional alpha/beta tag.</returns>
        public static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var parsedVersion = new FxA.Version(version.Major, version.Minor, version.MinorRevision);
            return parsedVersion.ToString();
        }

        /// <summary>
        /// Retrieves the numeric version number of this application (such as 0.4.1.0).
        /// </summary>
        /// <returns>Numeric version number with 4 16bit digits.</returns>
        public static string GetNumericVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var parsedVersion = string.Join(".", version.Major.ToString(), version.Minor.ToString(), version.MinorRevision.ToString(), version.Build.ToString());
            return parsedVersion;
        }

        /// <summary>
        /// Parses FxA URLs to be used within the application based on the provided base URL.
        /// </summary>
        /// <param name="customBaseURL">Base URL from which the remaining URLs will be constructed.</param>
        public static void LoadFxAUrls(string customBaseURL = "")
        {
            if (!string.IsNullOrEmpty(customBaseURL))
            {
                IsDevMode = true;
                BaseUrl = customBaseURL;
            }

            // FxA API link
            FxAUrl = string.Concat(BaseUrl, "/api/v1");

            // Login link
            FxALoginUrl = string.Concat(FxAUrl, "/vpn/login");

            // Feedback form link
            FeedbackFormUrl = string.Concat(BaseUrl, "/r/vpn/client/feedback", FxAUtmTags);

            // Subscription Link
            SubscriptionUrl = string.Concat(BaseUrl, "/vpn", FxAUtmTags);

            // Download Link
            DownloadUrl = string.Concat(BaseUrl, "/vpn/download", FxAUtmTags);

            // FxA account management URL
            FxAAccountManagementUrl = string.Concat(BaseUrl, "/r/vpn/account", FxAUtmTags);

            // Support URL
            SupportUrl = string.Concat("https://support.mozilla.org/1/vpn/", GetVersion(), "/Windows/en-US/vpn", FxAUtmTags);

            // Contact URL
            ContactUrl = string.Concat(BaseUrl, "/r/vpn/contact", FxAUtmTags);

            // Terms of service URL
            TermsUrl = string.Concat(BaseUrl, "/r/vpn/terms", FxAUtmTags);

            // Privacy policy URL
            PrivacyUrl = string.Concat(BaseUrl, "/r/vpn/privacy", FxAUtmTags);
        }
    }
}
