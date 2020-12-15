// <copyright file="MainWindowViewModel.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.ViewModels
{
    /// <summary>
    /// The main View Model class, used for presenting data to the UI.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Default Initial Server Country
        private const string DefaultServerCountry = "USA";

        // Connection Status Properties
        private readonly Models.ConnectionStatus connectionStatus;
        private readonly int initNumSpeeds = 30;
        private string connectionTime = "00:00:00";
        private string rx;
        private string tx;
        private Queue<double> downloadSpeedHistory;
        private Queue<double> uploadSpeedHistory;
        private string downloadSpeedHistoryString;
        private string uploadSpeedHistoryString;
        private string lastDownloadSpeed;
        private string lastUploadSpeed;
        private string lastDownloadSpeedUnits;
        private string lastUploadSpeedUnits;
        private bool isDownloadIdle;
        private bool isUploadIdle;
        private string lastHandshakeState;
        private Models.ConnectionState tunnelStatus = Models.ConnectionState.Unprotected;
        private bool isConnectionTransitioning;

        // Server List Properties
        private List<Models.CountryServerListItem> countryServerList;
        private Models.CityServerListItem serverCityListSelectedItem;
        private Models.ServerListItem serverSelected;

        // IP Information Properties
        private Models.IpInfo ipInfo;
        private string ipAddressString;

        // Device Management Properties
        private IEnumerable<Models.DeviceListItem> deviceList;
        private int userNumDevices;
        private int maxNumDevices;

        // Subscription Status Properties
        private string subscriptionStatus;

        // Server switching
        private string switchingServerFrom;
        private string switchingServerTo;
        private bool isServerSwitching;

        // Indicates whether the application has ran on startup
        private bool ranOnStartup;

        private bool newUserSignIn;

        // Language properties
        private List<CultureInfo> additionalLanguagesList;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize connectionStatus in the view model
            connectionStatus = new Models.ConnectionStatus
            {
                Status = Models.ConnectionState.Unprotected,
            };

            // Intialize the selected item of the server list
            RefreshServerListSelectedItem(initialLoad: true);

            InitializeSpeedLists();
            InitializeSpeedListStrings();

            // Get the number of user devices and current device if exists
            JSONStructures.Device currentDevice = null;

            if (Manager.Account.Config.FxALogin.User != null)
            {
                currentDevice = Manager.Account.Config.FxALogin.User.Devices.Find(d => d.PublicKey == Manager.Account.Config.FxALogin.PublicKey);
                UserNumDevices = Manager.Account.Config.FxALogin.User.Devices.Count();
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the initial frame view on application startup (UI).
        /// </summary>
        public Type InitialViewFrameSourceType { get; set; }

        /// <summary>
        /// Gets or sets the initial view control (UI).
        /// </summary>
        public UserControl ViewFrameSource { get; set; }

        /// <summary>
        /// Gets or sets the tunnel status. Can be protected or unprotected as long term states, or connecting/disconnecting as short term states.
        /// </summary>
        public Models.ConnectionState TunnelStatus
        {
            get
            {
                return tunnelStatus;
            }

            set
            {
                if (tunnelStatus != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Tunnel status changed from", tunnelStatus, "to", value);
                }

                switch (value)
                {
                    case Models.ConnectionState.Protected:
                        tunnelStatus = value;
                        break;
                    case Models.ConnectionState.Unprotected:
                        tunnelStatus = value;
                        break;
                    case Models.ConnectionState.Connecting:
                        if (!WireGuard.Connector.Connect())
                        {
                            tunnelStatus = Models.ConnectionState.Unprotected;
                        }
                        else
                        {
                            tunnelStatus = value;
                        }

                        break;
                    case Models.ConnectionState.Disconnecting:
                    default:
                        if (!WireGuard.Connector.Disconnect())
                        {
                            tunnelStatus = Models.ConnectionState.Protected;
                        }
                        else
                        {
                            tunnelStatus = value;
                        }

                        break;
                }

                OnPropertyChanged("TunnelStatus");
            }
        }

        /// <summary>
        /// Gets or sets the VPN connection status.
        /// </summary>
        public Models.ConnectionState Status
        {
            get
            {
                return connectionStatus.Status;
            }

            set
            {
                if (connectionStatus.Status != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Connection status changing from", connectionStatus.Status, "to", value);
                }

                connectionStatus.Status = value;
                OnPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application has been run on startup.
        /// </summary>
        public bool RanOnStartup
        {
            get
            {
                return ranOnStartup;
            }

            set
            {
                ranOnStartup = value;
            }
        }

        /// <summary>
        /// Gets or sets the tunnel status. Can be protected or unprotected as long term states, or connecting/disconnecting as short term states.
        /// </summary>
        public Models.ConnectionStability Stability
        {
            get
            {
                return connectionStatus.ConnectionStability;
            }

            set
            {
                if (connectionStatus.ConnectionStability != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Connection status changing from", connectionStatus.ConnectionStability, "to", value);
                }

                connectionStatus.ConnectionStability = value;
                OnPropertyChanged("Stability");
            }
        }

        /// <summary>
        /// Gets or sets the received bytes count for the current VPN connection.
        /// </summary>
        public string Rx
        {
            get
            {
                return rx;
            }

            set
            {
                rx = value;
                OnPropertyChanged("Rx");
            }
        }

        /// <summary>
        /// Gets or sets the transmitted bytes count for the current VPN connection.
        /// </summary>
        public string Tx
        {
            get
            {
                return tx;
            }

            set
            {
                tx = value;
                OnPropertyChanged("Tx");
            }
        }

        /// <summary>
        /// Gets or sets the list of recent download speeds.
        /// </summary>
        public Queue<double> DownloadSpeedHistory
        {
            get
            {
                return downloadSpeedHistory;
            }

            set
            {
                downloadSpeedHistory = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of recent upload speeds.
        /// </summary>
        public Queue<double> UploadSpeedHistory
        {
            get
            {
                return uploadSpeedHistory;
            }

            set
            {
                uploadSpeedHistory = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of recent download speeds as a string.
        /// </summary>
        public string DownloadSpeedHistoryString
        {
            get
            {
                return downloadSpeedHistoryString;
            }

            set
            {
                downloadSpeedHistoryString = value;
                OnPropertyChanged("DownloadSpeedHistoryString");
            }
        }

        /// <summary>
        /// Gets or sets the list of recent upload speeds as a string.
        /// </summary>
        public string UploadSpeedHistoryString
        {
            get
            {
                return uploadSpeedHistoryString;
            }

            set
            {
                uploadSpeedHistoryString = value;
                OnPropertyChanged("UploadSpeedHistoryString");
            }
        }

        /// <summary>
        /// Gets or sets the most recent download speed.
        /// </summary>
        public string LastDownloadSpeed
        {
            get
            {
                return lastDownloadSpeed;
            }

            set
            {
                lastDownloadSpeed = value;
                OnPropertyChanged("LastDownloadSpeed");
            }
        }

        /// <summary>
        /// Gets or sets the most recent upload speed.
        /// </summary>
        public string LastUploadSpeed
        {
            get
            {
                return lastUploadSpeed;
            }

            set
            {
                lastUploadSpeed = value;
                OnPropertyChanged("LastUploadSpeed");
            }
        }

        /// <summary>
        /// Gets or sets the list of recent upload speeds.
        /// </summary>
        public string LastDownloadSpeedUnits
        {
            get
            {
                return lastDownloadSpeedUnits;
            }

            set
            {
                lastDownloadSpeedUnits = value;
                OnPropertyChanged("LastDownloadSpeedUnits");
            }
        }

        /// <summary>
        /// Gets or sets the list of recent upload speeds.
        /// </summary>
        public string LastUploadSpeedUnits
        {
            get
            {
                return lastUploadSpeedUnits;
            }

            set
            {
                lastUploadSpeedUnits = value;
                OnPropertyChanged("LastUploadSpeedUnits");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether downloading is idle.
        /// </summary>
        public bool IsDownloadIdle
        {
            get
            {
                return isDownloadIdle;
            }

            set
            {
                isDownloadIdle = value;
                OnPropertyChanged("IsDownloadIdle");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets value indicating whether uploading is idle.
        /// </summary>
        public bool IsUploadIdle
        {
            get
            {
                return isUploadIdle;
            }

            set
            {
                isUploadIdle = value;
                OnPropertyChanged("IsUploadIdle");
            }
        }

        /// <summary>
        /// Gets or sets the connection time string (e.g. "00:00:00").
        /// </summary>
        public string ConnectionTime
        {
            get
            {
                return connectionTime;
            }

            set
            {
                connectionTime = value;
                OnPropertyChanged("ConnectionTime");
            }
        }

        /// <summary>
        /// Gets or sets the last handshake time retrieved from the WireGuard named pipe.
        /// </summary>
        public string LastHandshakeState
        {
            get
            {
                return lastHandshakeState;
            }

            set
            {
                lastHandshakeState = value;
                OnPropertyChanged("LastHandshakeState");
            }
        }

        /// <summary>
        /// Gets or sets the FxA subscription status.
        /// </summary>
        public string SubscriptionStatus
        {
            get
            {
                return subscriptionStatus;
            }

            set
            {
                if (subscriptionStatus != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Subscription status changing from", subscriptionStatus, "to", value);
                }

                subscriptionStatus = value;
                OnPropertyChanged("SubscriptionStatus");
            }
        }

        /// <summary>
        /// Gets or sets the list of countries to be used with the server list.
        /// </summary>
        public List<Models.CountryServerListItem> CountryServerList
        {
            get
            {
                return countryServerList;
            }

            set
            {
                countryServerList = value;
                OnPropertyChanged("CountryServerList");
            }
        }

        /// <summary>
        /// Gets or sets the server city item selected.
        /// </summary>
        public Models.CityServerListItem ServerCityListSelectedItem
        {
            get
            {
                return serverCityListSelectedItem;
            }

            set
            {
                serverCityListSelectedItem = value;
                OnPropertyChanged("ServerListSelectedItem");
            }
        }

        /// <summary>
        /// Gets or sets the servers in the selected city server list.
        /// </summary>
        public Models.ServerListItem ServerSelected
        {
            get
            {
                return serverSelected;
            }

            set
            {
                serverSelected = value;
                OnPropertyChanged("ServerSelected");
            }
        }

        /// <summary>
        /// Gets or sets the IP information.
        /// </summary>
        public Models.IpInfo IpInfo
        {
            get
            {
                return ipInfo;
            }

            set
            {
                ipInfo = value;
                OnPropertyChanged("IpInfo");
            }
        }

        /// <summary>
        /// Gets or sets the IP Address.
        /// </summary>
        public string IpAddressString
        {
            get
            {
                return ipAddressString;
            }

            set
            {
                ipAddressString = value;
                OnPropertyChanged("ipAddressString");
            }
        }

        /// <summary>
        /// Gets or sets the device list ("My Devices").
        /// </summary>
        public IEnumerable<Models.DeviceListItem> DeviceList
        {
            get
            {
                return deviceList;
            }

            set
            {
                deviceList = value;
                OnPropertyChanged("DeviceList");
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of devices one can have with their FxA FPN VPN account.
        /// </summary>
        public int MaxNumDevices
        {
            get
            {
                return maxNumDevices;
            }

            set
            {
                if (maxNumDevices != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("User's max num of allowed devices changed from", maxNumDevices, "to", value);
                }

                maxNumDevices = value;
                OnPropertyChanged("MaxNumDevices");
            }
        }

        /// <summary>
        /// Gets or sets the current number of devices one has in their FxA FPN VPN account.
        /// </summary>
        public int UserNumDevices
        {
            get
            {
                return userNumDevices;
            }

            set
            {
                if (userNumDevices != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("User's current number of devices changed from", userNumDevices, "to", value);
                }

                userNumDevices = value;
                OnPropertyChanged("UserNumDevices");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is transitioning. If the connection is transitioning, it means a connection or a disconnection is in progress.
        /// </summary>
        public bool IsConnectionTransitioning
        {
            get
            {
                return isConnectionTransitioning;
            }

            set
            {
                isConnectionTransitioning = value;
                OnPropertyChanged("IsConnectionTransitioning");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client is switching servers dynamically. If true, this indicates there's a server switch in progress.
        /// </summary>
        public bool IsServerSwitching
        {
            get
            {
                return isServerSwitching;
            }

            set
            {
                if (isServerSwitching != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Switching servers parameter changed from", isServerSwitching, "to", value);
                }

                isServerSwitching = value;
                OnPropertyChanged("IsServerSwitching");
            }
        }

        /// <summary>
        /// Gets or sets the origin server from which the client is transitioning. Used in conjuction with IsServerSwitching.
        /// </summary>
        public string SwitchingServerFrom
        {
            get
            {
                return switchingServerFrom;
            }

            set
            {
                if (switchingServerFrom != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Switching servers parameter 'from' has been changed from", switchingServerFrom, "to", value);
                }

                switchingServerFrom = value;
                OnPropertyChanged("SwitchingServerFrom");
            }
        }

        /// <summary>
        /// Gets or sets the destination server to which the client is transitioning. Used in conjuction with IsServerSwitching.
        /// </summary>
        public string SwitchingServerTo
        {
            get
            {
                return switchingServerTo;
            }

            set
            {
                if (switchingServerTo != value)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("Switching servers parameter 'To' has been changed To", switchingServerTo, "to", value);
                }

                switchingServerTo = value;
                OnPropertyChanged("SwitchingServerTo");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the client is in DEV mode. DEV mode is set when FxA URLs are overriden.
        /// </summary>
        public bool IsDevMode
        {
            get
            {
                return ProductConstants.IsDevMode;
            }
        }

        /// <summary>
        /// Sets a value indicating whether a user has just signed in.
        /// </summary>
        public bool NewUserSignIn
        {
            set
            {
                newUserSignIn = value;
                OnPropertyChanged("NewUserSignIn");
            }
        }

        /// <summary>
        /// Gets or sets the list of additional languages, based on the number of Fluent languages included with the application.
        /// </summary>
        public List<CultureInfo> AdditionalLanguagesList
        {
            get
            {
                return additionalLanguagesList;
            }

            set
            {
                additionalLanguagesList = value;
                OnPropertyChanged("AdditionalLanguagesList");
            }
        }

        /// <summary>
        /// Sets the latest selected item from the server list and sets it to serverListSelectedItem.
        /// </summary>
        /// <param name="initialLoad">Are we currently loading the.</param>
        public void RefreshServerListSelectedItem(bool initialLoad = false)
        {
            // Cannot set selected item if server list contains no servers
            if (FxA.Cache.FxAServerList.GetServerCitiesList().Count() == 0)
            {
                return;
            }

            // Set the selected item of the server list to the server specificed in the current WireGuard configuration.  If it doesn't exist, default to random server in US
            try
            {
                var previouslySelectedServerIndex = 0;

                if (initialLoad || newUserSignIn)
                {
                    newUserSignIn = false;

                    // Get the saved WireGuard configuration
                    var configuration = new WireGuard.Config(ProductConstants.FirefoxPrivateNetworkConfFile);
                    previouslySelectedServerIndex = FxA.Cache.FxAServerList.GetServerIndexByIP(configuration.GetPeerEndpointWithoutPort());
                }
                else
                {
                    // Get the selected server prior to server list refresh
                    previouslySelectedServerIndex = FxA.Cache.FxAServerList.GetServerIndexByIP(ServerSelected.Endpoint);
                }

                var selectedServerCity = FxA.Cache.FxAServerList.GetServerItems()[previouslySelectedServerIndex].City;
                ServerCityListSelectedItem = FxA.Cache.FxAServerList.GetServerCitiesList().FirstOrDefault(x => x.City == selectedServerCity);
            }
            catch (Exception)
            {
                Random rand = new Random();
                var serverCitiesInDefaultServerCounty = FxA.Cache.FxAServerList.GetServerCitiesList().Where(x => x.Country == DefaultServerCountry);

                if (serverCitiesInDefaultServerCounty.Count() > 0)
                {
                    ServerCityListSelectedItem = serverCitiesInDefaultServerCounty.ElementAt(rand.Next(0, serverCitiesInDefaultServerCounty.Count()));
                }
                else
                {
                    ServerCityListSelectedItem = FxA.Cache.FxAServerList.GetServerCitiesList()[rand.Next(0, FxA.Cache.FxAServerList.GetServerCitiesList().Count)];
                }
            }
            finally
            {
                UpdateServerSelection();
            }
        }

        /// <summary>
        /// Updates the selected server for connection.
        /// </summary>
        public void UpdateServerSelection()
        {
            var server = FxA.Cache.FxAServerList.SelectServer(ServerCityListSelectedItem);
            ServerSelected = ServerCityListSelectedItem.Servers.FirstOrDefault(x => x.Endpoint == server.Endpoint);
        }

        /// <summary>
        /// Propagates changed properties within this view model to notify other handlers.
        /// </summary>
        /// <param name="propertyName">Property name to propagate.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// Initializes list of 0's for initial speeds.
        /// </summary>
        /// <returns>List of speeds.</returns>
        private Queue<double> GetInitialSpeedList()
        {
            Queue<double> speeds = new Queue<double>();

            for (int i = 0; i < initNumSpeeds; i++)
            {
                speeds.Enqueue(0);
            }

            return speeds;
        }

        /// <summary>
        /// Initializes upload and download speed lists.
        /// </summary>
        private void InitializeSpeedLists()
        {
            DownloadSpeedHistory = GetInitialSpeedList();
            UploadSpeedHistory = GetInitialSpeedList();
        }

        /// <summary>
        /// Initializes list of 0's for initial speeds.
        /// </summary>
        /// <returns>List of speeds.</returns>
        private string GetInitialSpeedListString()
        {
            string speeds = string.Empty;

            for (int i = 0; i < initNumSpeeds; i++)
            {
                speeds += "0,";
            }

            return speeds;
        }

        /// <summary>
        /// Initializes upload and download speed lists.
        /// </summary>
        private void InitializeSpeedListStrings()
        {
            DownloadSpeedHistoryString = GetInitialSpeedListString();
            UploadSpeedHistoryString = GetInitialSpeedListString();
        }
    }
}
