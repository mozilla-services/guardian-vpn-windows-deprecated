// <copyright file="IService.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using FirefoxPrivateNetwork.JSONStructures;
using FirefoxPrivateNetwork.Models;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// WCF Service interface.
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// Default.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate ="/", ResponseFormat = WebMessageFormat.Json)]
        Response Index();

        /// <summary>
        /// Connect stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response Connect();

        /// <summary>
        /// Disconnect stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response Disconnect();

        /// <summary>
        /// Login stub.
        /// </summary>
        /// <param name="req">Login request object.</param>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response Login(LoginRequest req);

        /// <summary>
        /// Logout stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response Logout();

        /// <summary>
        /// Login state stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Response LoginState();

        /// <summary>
        /// Connection status stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Response ConnectionStatus();

        /// <summary>
        /// Account details stub.
        /// </summary>
        /// <returns>WCF user object response.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        User AccountDetails();

        /// <summary>
        /// Server list stub.
        /// </summary>
        /// <returns>List of server list items.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        List<ServerListItem> ServerList();

        /// <summary>
        /// Device list stub.
        /// </summary>
        /// <returns>List of Device JSON objects.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        List<Device> DeviceList();

        /// <summary>
        /// Add device stub.
        /// </summary>
        /// <param name="device">Device request object.</param>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response AddDevice(DeviceRequest device);

        /// <summary>
        /// Remove device stub.
        /// </summary>
        /// <param name="device">Device request object.</param>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response RemoveDevice(DeviceRequest device);

        /// <summary>
        /// Version check stub.
        /// </summary>
        /// <param name="req">Version request object.</param>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Task<BalrogResponse> VersionCheck(VersionRequest req);

        /// <summary>
        /// Subscription check stub.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response SubscriptionCheck();

        /// <summary>
        /// Process check stub.
        /// </summary>
        /// <returns>WCF Process Check response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ProcessCheckResponse FirefoxPrivateNetworkProcessCheck();

        /// <summary>
        /// Update Root Fingerprint.
        /// </summary>
        /// <param name="req">Root Fingerprint request.</param>
        /// <returns>WCF Response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Response UpdateRootFingerprint(RootFingerprintRequest req);

        /// <summary>
        /// Download MSI and update.
        /// </summary>
        /// <param name="req">Version request object.</param>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Task<Response> DownloadMSIAndUpdate(VersionRequest req);

        /// <summary>
        /// Close WCF server.
        /// </summary>
        /// <returns>WCF response.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        Response CloseConnection();
    }
}
