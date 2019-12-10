// <copyright file="UpdateHttpClient.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.Update
{
    /// <summary>
    /// Client used to send HTTP requests to the Balrog remote infrastructure.
    /// </summary>
    internal class UpdateHttpClient : IDisposable
    {
        private HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHttpClient"/> class.
        /// </summary>
        /// <param name="maxDownloadSizeBytes">Maximum response content buffer size.</param>
        public UpdateHttpClient(int maxDownloadSizeBytes)
        {
            httpClient = new HttpClient()
            {
                MaxResponseContentBufferSize = maxDownloadSizeBytes,
            };

            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                MustRevalidate = true,
                NoStore = true,
            };

            httpClient.DefaultRequestHeaders.ConnectionClose = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            httpClient.Dispose();
        }

        /// <summary>
        /// Queries the url with retry logic.
        /// </summary>
        /// <param name="url">The url to query.</param>
        /// <param name="maxRetries">Maximum number of rety attempts.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> QueryWithRetryAsync(string url, int maxRetries)
        {
            HttpResponseMessage response = null;

            // No caching
            for (var attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    response = await httpClient.GetAsync(url);
                    break;
                }
                catch (HttpRequestException e)
                {
                    ErrorHandling.ErrorHandler.Handle(string.Format("Failed to get response from the following url: {0}. Retry attempt: {1}.", url, attempt), ErrorHandling.LogLevel.Error);

                    if (attempt == maxRetries)
                    {
                        throw e;
                    }
                }
            }

            return response;
        }
    }
}
