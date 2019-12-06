// <copyright file="Tester.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace FirefoxPrivateNetwork.WCF
{
    /// <summary>
    /// Tester helper class which opens up HTTP endpoints for test querying.
    /// </summary>
    public class Tester
    {
        private static WebServiceHost host;

        /// <summary>
        /// Opens a port on the local machine and listens for requests.
        /// </summary>
        [Conditional("DEBUG_QA")]
        public static void OpenConnection()
        {
            host = new WebServiceHost(typeof(Service), new Uri("http://127.0.0.1:8000/"));
            try
            {
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), string.Empty);
                host.Open();
                ChannelFactory<IService> cf = new ChannelFactory<IService>(new WebHttpBinding(), "http://127.0.0.1:8000");
                cf.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
                IService channel = cf.CreateChannel();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("An exception occurred: {0}", ex.Message);
                host.Abort();
            }
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        [Conditional("DEBUG_QA")]
        public static void CloseConnection()
        {
            try
            {
                CommunicationState[] closedStates = { CommunicationState.Closing, CommunicationState.Closed };
                if (!Array.Exists(closedStates, state => state == host.State))
                {
                    host.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: {0}", ex.Message);
            }
        }
    }
}
