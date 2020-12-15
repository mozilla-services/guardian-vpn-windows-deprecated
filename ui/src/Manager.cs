// <copyright file="Manager.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FirefoxPrivateNetwork
{
    /// <summary>
    /// Holds instantiations of essential app elements.
    /// </summary>
    internal class Manager
    {
        /// <summary>
        /// Gets or sets the application tray icon handler.
        /// </summary>
        public static NotificationArea.Tray TrayIcon { get; set; }

        /// <summary>
        /// Gets or sets the WireGuard tunnel service manager.
        /// </summary>
        public static WireGuard.Tunnel Tunnel { get; set; }

        /// <summary>
        /// Gets the login session manager that can help to start/cancel a login session.
        /// </summary>
        public static FxA.LoginSessionManager LoginSessionManager { get; private set; } = new FxA.LoginSessionManager();

        /// <summary>
        /// Gets or sets the user's FxA account.
        /// </summary>
        public static FxA.Account Account { get; set; }

        /// <summary>
        /// Gets or sets the view model for the main window.
        /// </summary>
        public static ViewModels.MainWindowViewModel MainWindowViewModel { get; set; }

        /// <summary>
        /// Gets or sets the wireless local area network watcher.
        /// </summary>
        public static Network.WlanWatcher WlanWatcher { get; set; }

        /// <summary>
        /// Gets or sets the captive portal detector.
        /// </summary>
        public static Network.CaptivePortalDetection CaptivePortalDetector { get; set; }

        /// <summary>
        /// Gets or sets the network pinger.
        /// </summary>
        public static Network.Pinger PingManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a mandatory application update is required.
        /// </summary>
        public static bool MustUpdate { get; set; } = false;

        /// <summary>
        /// Gets or sets the tray message handler window.
        /// </summary>
        public static NotificationArea.TrayMessageWindow TrayMessageWindow { get; set; }

        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        public static Settings Settings { get; set; }

        /// <summary>
        /// Initialize all elements of the Manager class.
        /// </summary>
        public static void Initialize()
        {
            InitializeSettings();
            InitializeTray();
            InitializeServerListCache();
            InitializeTunnel();
            InitializeAccount();
            InitializeViewModels();
            InitializeWlanWatcher();
            InitializeCaptivePortalDetector();
        }

        /// <summary>
        /// Initialize the Broker if not already initialized and then initialize the tunnel class.
        /// </summary>
        public static void InitializeTunnel()
        {
            if (Tunnel == null)
            {
                Tunnel = new WireGuard.Tunnel();
            }
        }

        /// <summary>
        /// Initialize the system tray icon of not already initialized.
        /// </summary>
        public static void InitializeTray()
        {
            TrayMessageWindow = new NotificationArea.TrayMessageWindow(ProductConstants.TrayWindowClassName);

            if (TrayMessageWindow.GetHandle() == IntPtr.Zero)
            {
                return;
            }

            if (TrayIcon == null)
            {
                TrayIcon = new NotificationArea.Tray(TrayMessageWindow);
            }
        }

        /// <summary>
        /// Initialize the server list cache.
        /// </summary>
        public static void InitializeServerListCache()
        {
            FxA.Cache.FxAServerList = new FxA.ServerList();
            FxA.Cache.FxAServerList.LoadServerDataFromFile(Path.Combine(ProductConstants.UserAppDataFolder, "servers.json"));
        }

        /// <summary>
        /// Initialize the main window view model.
        /// </summary>
        public static void InitializeViewModels()
        {
            MainWindowViewModel = new ViewModels.MainWindowViewModel();
        }

        /// <summary>
        /// Initialize the wireless local area network watcher.
        /// </summary>
        public static void InitializeWlanWatcher()
        {
            WlanWatcher = new Network.WlanWatcher();
        }

        /// <summary>
        /// Initialize the captive portal detector.
        /// </summary>
        public static void InitializeCaptivePortalDetector()
        {
            CaptivePortalDetector = new Network.CaptivePortalDetection();
        }

        /// <summary>
        /// Initializes the user's FxA account.
        /// </summary>
        public static void InitializeAccount()
        {
            Account = new FxA.Account();
        }

        /// <summary>
        /// Initializes the application settings.
        /// </summary>
        public static void InitializeSettings()
        {
            var migSettings = new Migrations.Settings();
            migSettings.MigrateConfigAddressToSettingsFile();
            Settings = new Settings(ProductConstants.SettingsFile);
        }
    }
}
