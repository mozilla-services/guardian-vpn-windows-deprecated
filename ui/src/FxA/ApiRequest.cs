// <copyright file="ApiRequest.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Cache;
using Newtonsoft.Json;
using RestSharp;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Wrapper for REST requests to the FxA API.
    /// </summary>
    internal class ApiRequest
    {
        private RestRequest request;
        private RestClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequest"/> class.
        /// </summary>
        /// <param name="token">Access token to API.</param>
        /// <param name="url">API endpoint.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="caching">Indicates whether caching is enabled for requests.</param>
        public ApiRequest(string token, string url, RestSharp.Method method = RestSharp.Method.GET, bool caching = true)
        {
            client = new RestClient(ProductConstants.FxAUrl);
            request = new RestRequest(url, method);

            client.UserAgent = ProductConstants.GetUserAgent();

            if (!caching)
            {
                client.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                request.AddHeader("Connection", "close");
            }

            request.AddParameter("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;
        }

        /// <summary>
        /// Adds key/value pairs to the request JSON body for POST method.
        /// </summary>
        /// <param name="parameters">Key/value pairs to be added to the JSON body.</param>
        public void AddPostBody(Dictionary<string, string> parameters)
        {
            foreach (var p in parameters)
            {
                request.AddJsonBody(JsonConvert.SerializeObject(parameters, Formatting.Indented));
            }
        }

        /// <summary>
        /// Adds key/value pairs to the GET method URL parameters.
        /// </summary>
        /// <param name="parameters">Key/value pairs to be added to the URL parameters.</param>
        public void AddGetParameters(Dictionary<string, string> parameters)
        {
            foreach (var p in parameters)
            {
                request.AddParameter(p.Key, p.Value);
            }
        }

        /// <summary>
        /// Gets the REST request instance.
        /// </summary>
        /// <returns>A <see cref="RestRequest"/> object.</returns>
        public RestRequest GetRequestInstance()
        {
            return request;
        }

        /// <summary>
        /// Sends a REST request.
        /// </summary>
        /// <returns>Returns a REST response.</returns>
        public IRestResponse SendRequest()
        {
            IRestResponse response = null;

            try
            {
                // Execute the request
                response = client.Execute(request);

                // Empty status response
                if (response.StatusCode == 0 || response.Content == null)
                {
                    ErrorHandling.DebugLogger.LogDebugMsg("res=" + request.Resource +
                        ", status=" + response.ResponseStatus + ", msg=" + response.ErrorMessage);
                    return null;
                }

                // Parse the JSON response if response status code is within the success range
                if (response.StatusCode < System.Net.HttpStatusCode.OK || response.StatusCode >= System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = JsonConvert.DeserializeObject<JSONStructures.ApiError>(response.Content);
                    ErrorHandling.ErrorHandler.Handle(errorResponse.ToString(), ErrorHandling.LogLevel.Error);
                }

                return response;
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e.Message + "\n" + e.ToString() + "\n" + e.StackTrace, ErrorHandling.LogLevel.Error);
            }

            return response;
        }
    }
}
