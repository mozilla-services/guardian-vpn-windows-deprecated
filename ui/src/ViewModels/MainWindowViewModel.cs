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
        // Connection Status Properties
        private readonly Models.ConnectionStatus connectionStatus;
        private string connectionTime = "00:00:00";
        private string rxTx;
        private string lastHandshakeState;
        private Models.ConnectionState tunnelStatus = Models.ConnectionState.Unprotected;
        private bool isConnectionTransitioning;

        // Server List Properties
        private List<Models.CountryServerListItem> countryServerList;
        private Models.ServerListItem serverListSelectedItem;

        // Device Management Properties
        private IEnumerable<Models.DeviceListItem> deviceList;
        private int userNumDevices;
        private int maxNumDevices;

        // Version Update Properties
        private UI.Components.Toast.Toast updateToast;

        // Subscription Status Properties
        private string subscriptionStatus;

        // Server switching
        private string switchingServerFrom;
        private string switchingServerTo;
        private bool isServerSwitching;

        // Indicates whether the application has ran on startup
        private bool ranOnStartup;

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

            // Get the number of user devices and current device if exists
            JSONStructures.Device currentDevice = null;

            if (Manager.Account.Config.FxALogin.User != null)
            {
                currentDevice = Manager.Account.Config.FxALogin.User.Devices.Find(d => d.PublicKey == Manager.Account.Config.FxALogin.PublicKey);
                UserNumDevices = Manager.Account.Config.FxALogin.User.Devices.Count();
            }

            // Determine initial page to display in the ViewFrame
            if (Manager.Account.LoginState == FxA.LoginState.LoggedIn && currentDevice != null)
            {
                InitialViewFrameSourceType = typeof(UI.MainView);
            }
            else
            {
                InitialViewFrameSourceType = typeof(UI.LandingView);
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
        /// Gets or sets the updater toast message.
        /// </summary>
        public UI.Components.Toast.Toast UpdateToast
        {
            get
            {
                return updateToast;
            }

            set
            {
                updateToast = value;
                OnPropertyChanged("UpdateToast");
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
        /// Gets or sets the received/transmitted bytes count for the current VPN connection.
        /// </summary>
        public string RxTx
        {
            get
            {
                return rxTx;
            }

            set
            {
                rxTx = value;
                OnPropertyChanged("RxTx");
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
        /// Gets or sets the servers in the server list.
        /// </summary>
        public Models.ServerListItem ServerListSelectedItem
        {
            get
            {
                return serverListSelectedItem;
            }

            set
            {
                serverListSelectedItem = value;
                OnPropertyChanged("ServerListSelectedItem");
            }
        }

        /// <summary>
        /// Gets the server list count.
        /// </summary>
        public int ServerListCount
        {
            get
            {
                return FxA.Cache.FxAServerList.GetServerList().Count();
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
        /// Gets the translation matching the specified name for localization.
        /// </summary>
        /// <param name="name">Index name/key of the translation to access.</param>
        public string this[string name]
        {
            get
            {
                return Manager.TranslationService.GetString(name);
            }
        }

        /// <summary>
        /// Sets the latest selected item from the server list and sets it to serverListSelectedItem.
        /// </summary>
        /// <param name="initialLoad">Are we currently loading the.</param>
        public void RefreshServerListSelectedItem(bool initialLoad = false)
        {
            // Cannot set selected item if server list contains no servers
            if (FxA.Cache.FxAServerList.GetServerList().Count() == 0)
            {
                return;
            }

            // Set the selected item of the server list to the server specificed in the current WireGuard configuration.  If it doesn't exist, default to the first server
            try
            {
                var previouslySelectedServerIndex = 0;

                if (initialLoad)
                {
                    // Get the saved WireGuard configuration
                    var configuration = new WireGuard.Config(ProductConstants.FirefoxPrivateNetworkConfFile);
                    previouslySelectedServerIndex = FxA.Cache.FxAServerList.GetServerIndexByIP(configuration.GetPeerEndpointWithoutPort());
                }
                else
                {
                    // Get the selected server prior to server list refresh
                    previouslySelectedServerIndex = FxA.Cache.FxAServerList.GetServerIndexByIP(serverListSelectedItem.Endpoint);
                }

                serverListSelectedItem = FxA.Cache.FxAServerList.GetServerList()[previouslySelectedServerIndex];
            }
            catch (Exception)
            {
                serverListSelectedItem = FxA.Cache.FxAServerList.GetServerList()[0];
            }
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
    }
}