// <copyright file="Service.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FirefoxPrivateNetwork.JSONStructures;
using FirefoxPrivateNetwork.Models;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// WCF service interface.
    /// </summary>
    public class Service : IService
    {
        /// <summary>
        /// Indicates that WPF is running.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response Index()
        {
            return new Response(200, "Service is running!");
        }

        /// <summary>
        /// Retrieve account details for the user.
        /// </summary>
        /// <returns>User details object.</returns>
        public User AccountDetails()
        {
            return new FxA.Account().GetAccountDetails();
        }

        /// <summary>
        /// Initiate connection.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response Connect()
        {
            try
            {
                if (Manager.Tunnel.ConnectionStatus().Status == Models.ConnectionState.Unprotected)
                {
                    var configuration = new WireGuard.Config(ProductConstants.FirefoxPrivateNetworkConfFile);

                    var currentServer = FxA.Cache.FxAServerList.GetServerByIndex(0);
                    configuration.SetEndpoint(FxA.Cache.FxAServerList.GetServerIPByIndex(0), FxA.Cache.FxAServerList.GetServerPublicKeyByIndex(0), ProductConstants.AllowedIPs, currentServer.DNSServerAddress);

                    bool result = Manager.Tunnel.Connect();
                    return new Response(result ? 200 : 500, result ? "successfully connect!" : "fail to connect");
                }

                return new Response(200, "successfully connect!");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Retrieve connection status.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response ConnectionStatus()
        {
            try
            {
                ConnectionState connectionState = Manager.Tunnel.ConnectionStatus().Status;
                return new Response(200, connectionState.ToString());
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieve device list.
        /// </summary>
        /// <returns>List of Device objects.</returns>
        public List<Device> DeviceList()
        {
            return new FxA.Account().GetAccountDevices();
        }

        /// <summary>
        /// Initiate disconnect.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response Disconnect()
        {
            try
            {
                Manager.Tunnel.Disconnect();
                return new Response(200, "successfully disconnect!");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Initiate login.
        /// </summary>
        /// <param name="req">LoginRequest WCF command object.</param>
        /// <returns>WCF response.</returns>
        public Response Login(LoginRequest req)
        {
            try
            {
                var loginInstance = new FxA.Login();
                var pollInterval = req.PollInterval % 31; // Max 30 seconds, no more
                Manager.Account.LoginState = FxA.LoginState.LoggingIn;
                loginInstance.StartQueryLoginThread(req.VerificationUrl, req.PollInterval, req.ExpiresOn);
                return new Response(200, "Success");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Initiate logout.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response Logout()
        {
            try
            {
                Manager.Account.Logout();
                return new Response(200, "Success");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieve server list.
        /// </summary>
        /// <returns>List of ServerListItems.</returns>
        public List<ServerListItem> ServerList()
        {
            return FxA.Cache.FxAServerList.GetServerList();
        }

        /// <summary>
        /// Add device.
        /// </summary>
        /// <param name="req">WCF device request.</param>
        /// <returns>WCF response.</returns>
        public Response AddDevice(DeviceRequest req)
        {
            try
            {
                var devices = new FxA.Devices();
                var addDeviceResponse = devices.AddDevice(req.DeviceName, req.PublicKey);
                return new Response(200, "Success");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Remove device.
        /// </summary>
        /// <param name="req">WCF device request.</param>
        /// <returns>WCF response.</returns>
        public Response RemoveDevice(DeviceRequest req)
        {
            try
            {
                var devices = new FxA.Devices();
                devices.RemoveDevice(req.PublicKey, false, true);
                return new Response(200, "Success");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Get the current login state.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response LoginState()
        {
            return new Response(200, Manager.Account.LoginState.ToString());
        }

        /// <summary>
        /// Starts the version check call.
        /// </summary>
        /// <param name="req">WCF version request.</param>
        /// <returns>Balrog Version response.</returns>
        public async Task<BalrogResponse> VersionCheck(VersionRequest req)
        {
            return await Update.Balrog.QueryUpdate(req.CurrentVersion);
        }

        /// <summary>
        /// Subscription check call.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response SubscriptionCheck()
        {
            try
            {
                Manager.AccountInfoUpdater.ForcePollAccountInfo().Wait();
                return new Response(200, Manager.Account.LoginState.ToString());
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message);
            }
        }

        /// <summary>
        /// Checks for running FPVPN processes and returns an empty response if there was an error.
        /// </summary>
        /// <returns>WCF process check response.</returns>
        public ProcessCheckResponse FirefoxPrivateNetworkProcessCheck()
        {
            try
            {
                return CheckFirefoxPrivateNetworkProcess();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: %s", ex.Message);

                return new ProcessCheckResponse(false, false, false, string.Empty);
            }
        }

        /// <summary>
        /// Update Root Fingerprint for Balrog.
        /// </summary>
        /// <param name="req">Root Fingerprint Request.</param>
        /// <returns>WCF response.</returns>
        public Response UpdateRootFingerprint(RootFingerprintRequest req)
        {
#if DEBUG_QA
            Update.Balrog.UpdateRootFingerprint = req.RootFingerprint;
#endif
            return new Response(200, "Success");
        }

        /// <summary>
        /// Download MSI and update.
        /// </summary>
        /// <param name="req">Version request object.</param>
        /// <returns>WCF response.</returns>
        public async Task<Response> DownloadMSIAndUpdate(VersionRequest req)
        {
            try
            {
                var result = await Update.Update.Run(req.CurrentVersion);
                return result == Update.Update.UpdateResult.Success ? new Response(200, "Success") : new Response(500, "Fail");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Close WCF server.
        /// </summary>
        /// <returns>WCF response.</returns>
        public Response CloseConnection()
        {
            try
            {
                Tester.CloseConnection();
                return new Response(200, "Success");
            }
            catch (Exception ex)
            {
                return new Response(500, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Checks for running FPVPN processes.
        /// </summary>
        /// <returns>WCF process check response.</returns>
        private ProcessCheckResponse CheckFirefoxPrivateNetworkProcess()
        {
            string processName = "FirefoxPrivateNetwork";
            bool uiExist = false;
            bool brokerExist = false;
            bool tunnelExist = false;
            StringBuilder commands = new StringBuilder();
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(processName))
                {
                    string commandLine = GetCommandLine(clsProcess).Trim();
                    commands.AppendLine(commandLine);
                    if (!uiExist)
                    {
                        Regex rgx = new Regex(@"^.*FirefoxPrivateNetworkVPN\.exe""$");
                        uiExist = rgx.IsMatch(commandLine);
                    }

                    if (!brokerExist)
                    {
                        Regex rgx = new Regex(@"^.*FirefoxPrivateNetworkVPN\.exe""\ broker.*$");
                        brokerExist = rgx.IsMatch(commandLine);
                    }

                    if (!tunnelExist)
                    {
                        Regex rgx = new Regex(@"^.*FirefoxPrivateNetworkVPN\.exe""\ tunnel.*$");
                        tunnelExist = rgx.IsMatch(commandLine);
                    }
                }
            }

            return new ProcessCheckResponse(uiExist, brokerExist, tunnelExist, commands.ToString());
        }

        /// <summary>
        /// Gets command line of process.
        /// </summary>
        /// <param name="process">Process to fetch the command line of.</param>
        /// <returns>Command line.</returns>
        private string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }
    }
}
