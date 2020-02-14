// <copyright file="Account.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FirefoxPrivateNetwork.ErrorHandling;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// FxA Account login states.
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// User is logged in.
        /// </summary>
        LoggedIn,

        /// <summary>
        /// User is logged out.
        /// </summary>
        LoggedOut,

        /// <summary>
        /// User is in the process of logging in.
        /// </summary>
        LoggingIn,
    }

    /// <summary>
    /// Represents a Fxa Account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
            // Try to load existing login session files from user's appdata folder
            if (Config.LoadFxAToken() && Config.LoadFxAUserFromFile(ProductConstants.FxAUserFile))
            {
                LoginState = LoginState.LoggedIn;
            }
        }

        /// <summary>
        /// Gets or sets the config structure associated with this account.
        /// </summary>
        public Config Config { get; set; } = new Config();

        /// <summary>
        /// Gets or sets the current login state indicating whether the user is logged in, is in the process of logging in or logged out.
        /// </summary>
        public LoginState LoginState { get; set; } = LoginState.LoggedOut;

        /// <summary>
        /// Processes login response from the FxA API and adds a new Account device.
        /// </summary>
        /// <param name="fxaJson">JSON string received from FxA containing login data.</param>
        /// <returns>True on successful processing of the login response.</returns>
        public bool ProcessLogin(string fxaJson)
        {
            // Initialize a new login session configuration
            Manager.Account.Config = new FxA.Config(fxaJson);

            // Sets a value that a user has just logged in
            Manager.MainWindowViewModel.NewUserSignIn = true;

            // Generate a new WireGuard keypair in preparation for adding a new Account device
            var keys = WireGuard.Keypair.Generate();
            Manager.Account.Config.FxALogin.PublicKey = keys.Public;

            // Save login session to user's appdata folder
            Manager.Account.Config.SaveFxAToken();
            Manager.Account.Config.WriteFxAUserToFile(ProductConstants.FxAUserFile);

            // Set the account login state to logged in
            Manager.Account.LoginState = FxA.LoginState.LoggedIn;

            // Initialize cache for avatar image
            Manager.InitializeCache();

            // Added a new account device through the FxA API, using the newly generated keypair
            var devices = new FxA.Devices();
            var deviceName = string.Format("{0} ({1} {2})", System.Environment.MachineName, System.Environment.OSVersion.Platform, System.Environment.OSVersion.Version);
            var deviceAddResponse = devices.AddDevice(deviceName, keys.Public);

            // Upon successful addition of a new device, save the device interface to the WireGuard configuration file and IP addresses to settings file
            if (deviceAddResponse != null)
            {
                var conf = new WireGuard.Config(keys.Private, deviceAddResponse.IPv4Address + "," + deviceAddResponse.IPv6Address, string.Empty);
                conf.WriteToFile(ProductConstants.FirefoxPrivateNetworkConfFile);

                var networkSettings = Manager.Settings.Network;
                networkSettings.IPv4Address = deviceAddResponse.IPv4Address;
                networkSettings.IPv6Address = deviceAddResponse.IPv6Address;
                Manager.Settings.Network = networkSettings;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Logs the user out, removes their current device if the removeDevice flag is set, and handles local files cleanup.
        /// Note: Removing a device will also effectively invalidate the user's current FxA token.
        /// </summary>
        /// <param name="removeDevice">Optional: Indicates the removal of the current user's device when set to true.</param>
        public void Logout(bool removeDevice = false)
        {
            try
            {
                // Disconnect the VPN tunnel
                Manager.Tunnel.Disconnect();

                // Remove the current account device
                if (removeDevice)
                {
                    var devices = new FxA.Devices();
                    devices.RemoveDevice(Manager.Account.Config.FxALogin.PublicKey, silent: true);
                }
            }
            catch (Exception)
            {
                ErrorHandler.Handle(new UserFacingMessage("toast-remove-device-error"), UserFacingErrorType.Toast, UserFacingSeverity.ShowWarning, LogLevel.Debug);
            }
            finally
            {
                // Clear up login session files from user's appdata folder
                Config.RemoveFxAToken();
                File.Delete(ProductConstants.FxAUserFile);
                File.Delete(ProductConstants.FirefoxPrivateNetworkConfFile);

                // Clear the cache for avatar image
                Manager.ClearCache();

                // Set logged out state and terminate UI Updater threads
                LoginState = LoginState.LoggedOut;
                Manager.TerminateUIUpdaters();
            }
        }

        /// <summary>
        /// Query the Fxa API for account details.
        /// </summary>
        /// <returns>FxA user JSON object.</returns>
        public JSONStructures.User GetAccountDetails()
        {
            var api = new ApiRequest(Manager.Account.Config.FxALogin.Token, "/vpn/account", RestSharp.Method.GET);

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorHandler.Handle("Could not retrieve account info.", LogLevel.Error);
                return null;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Manager.Account.Logout(removeDevice: false);
                return null;
            }

            try
            {
                Config.FxALogin.User = JsonConvert.DeserializeObject<JSONStructures.User>(response.Content);
                Manager.Account.Config.WriteFxAUserToFile(ProductConstants.FxAUserFile);

                return Config.FxALogin.User;
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e, LogLevel.Error);
            }

            return null;
        }

        /// <summary>
        /// Async task for GetAccountDetails.
        /// </summary>
        /// <param name="tcs">Completion indicator to set once done.</param>
        /// <returns>Returns an asynchronous operation in the form of a Task.</returns>
        public Task<bool> GetAccountDetailsTask(TaskCompletionSource<bool> tcs)
        {
            Task.Run(() =>
            {
                GetAccountDetails();
                if (!tcs.Task.IsCompleted)
                {
                    tcs.SetResult(true);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Retrieves all devices associated with the account.
        /// </summary>
        /// <returns>A list of FxA devices in the form of a JSON object.</returns>
        public List<JSONStructures.Device> GetAccountDevices()
        {
            var accountDetails = GetAccountDetails();

            if (accountDetails == null)
            {
                return null;
            }

            return accountDetails.Devices;
        }
    }
}
