// <copyright file="IpInfo.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirefoxPrivateNetwork.ErrorHandling;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Gets the IP info for the client.
    /// </summary>
    internal static class IpInfo
    {
        /// <summary>
        /// Detects the currently used public IP address and ISP location.
        /// </summary>
        /// <returns>IP address and location info.</returns>
        public static Models.IpInfo RetrieveIpInfo()
        {
            Models.IpInfo ipInfo = new Models.IpInfo();

            var api = new ApiRequest(Manager.Account.Config.FxALogin.Token, "/vpn/ipinfo", RestSharp.Method.GET, false);
            var response = api.SendRequest();

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorHandler.Handle("Could not retrieve IP info.", LogLevel.Error);
                return null;
            }

            try
            {
                JSONStructures.IpInfo info = JsonConvert.DeserializeObject<JSONStructures.IpInfo>(response.Content);
                ipInfo.City = info.City;
                ipInfo.Country = info.Country;
                ipInfo.Ip = info.Ip;
                ipInfo.Subregion = info.Subregion;

                return ipInfo;
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e, LogLevel.Error);
            }

            return null;
        }
    }
}
