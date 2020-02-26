// <copyright file="Utils.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using OpenQA.Selenium.Appium.Windows;
    using Polly;
    using RestSharp;

    /// <summary>
    /// Here are some utilities.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Random select an index from integers based on the condition.
        /// </summary>
        /// <param name="range">A range of integers.</param>
        /// <param name="condition">A condition method.</param>
        /// <returns>A random index.</returns>
        public static int RandomSelectIndex(IEnumerable<int> range, Func<int, bool> condition)
        {
            var result = range.Where(condition);
            var rand = new Random();
            int index = rand.Next(0, result.Count());
            return result.ElementAt(index);
        }

        /// <summary>
        /// Remove all space, tab, new line character from multiline string.
        /// </summary>
        /// <param name="text">A string text.</param>
        /// <returns>A new string text.</returns>
        public static string CleanText(string text)
        {
            return Regex.Replace(text, @"[\r\n\t\s]+", string.Empty);
        }

        /// <summary>
        /// Wait until the file exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="timeOut">The timeout.</param>
        /// <returns>File exists or not.</returns>
        public static bool WaitUntilFileExist(string path, double timeOut = 10000)
        {
            bool exist = false;
            WaitUntil(ref exist, File.Exists, path, (ex) => ex, timeOut);
            return exist;
        }

        /// <summary>
        /// Wait for {timeOut} milliseconds until find element.
        /// </summary>
        /// <param name="findMethod">The method used to find element.</param>
        /// <param name="selector">The selector used in the findMethod.</param>
        /// <param name="timeOut">Time out in milliseconds. Default is 10000 milliseconds.</param>
        /// <returns>Windows element.</returns>
        public static WindowsElement WaitUntilFindElement(Func<string, WindowsElement> findMethod, string selector, double timeOut = 10000)
        {
            WindowsElement element = null;
            Utils.WaitUntil(ref element, findMethod, selector, (ele) => ele != null, timeOut);
            return element;
        }

        /// <summary>
        /// Generic method to implement wait until.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="result">The reference to the result.</param>
        /// <param name="func">A function which takes string as parameter and return T.</param>
        /// <param name="param">The parameter passed to func.</param>
        /// <param name="condition">A function which takes result as parameter and return bool.</param>
        /// <param name="timeOut">The time out, default 10000 ms.</param>
        public static void WaitUntil<T>(ref T result, Func<string, T> func, string param, Func<T, bool> condition, double timeOut = 10000)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            bool retry = true;
            while (retry && time.ElapsedMilliseconds <= timeOut)
            {
                try
                {
                    result = func(param);
                    if (condition(result))
                    {
                        retry = false;
                        time.Stop();
                    }
                    else
                    {
                        retry = true;
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    }
                }
                catch (Exception)
                {
                    retry = true;
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }

            if (time.ElapsedMilliseconds > timeOut)
            {
                time.Stop();
            }

            time.Reset();
        }

        /// <summary>
        /// Send API request to check whether user is connected or not.
        /// </summary>
        /// <returns>The API response.</returns>
        /// <param name="expectedContent">Expected content returned from API.</param>
        public static IRestResponse AmIMullvad(string expectedContent = null)
        {
            var client = new RestClient(Constants.AmIMullvadConnectedAPI);
            var request = new RestRequest(Method.GET);
            Func<IRestResponse, bool> condition = (res) =>
            {
                if (string.IsNullOrEmpty(expectedContent))
                {
                    return res.StatusCode != HttpStatusCode.OK;
                }

                return res.StatusCode != HttpStatusCode.OK || !res.Content.Contains(expectedContent);
            };
            IRestResponse response = RetryExecute(client, request, condition);
            return response;
        }

        /// <summary>
        /// Call Mullvad API to get current city.
        /// </summary>
        /// <returns>The API response.</returns>
        /// <param name="expectedCity">Expected city returned from API.</param>
        public static IRestResponse GetCityViaMullvad(string expectedCity = null)
        {
            var client = new RestClient(Constants.AmIMullvadCityAPI);
            var request = new RestRequest(Method.GET);
            Func<IRestResponse, bool> condition = (res) =>
            {
                if (string.IsNullOrEmpty(expectedCity))
                {
                    return res.StatusCode != HttpStatusCode.OK;
                }

                return res.StatusCode != HttpStatusCode.OK || !expectedCity.Contains(res.Content.Trim());
            };
            IRestResponse response = RetryExecute(client, request, condition);
            Console.WriteLine($"Mullvad API city response: {response.Content}");
            return response;
        }

        /// <summary>
        /// Execute the request with retry policy.
        /// </summary>
        /// <param name="client">RestClient object.</param>
        /// <param name="request">RestRequest object.</param>
        /// <returns>The API response.</returns>
        private static IRestResponse RetryExecute(IRestClient client, IRestRequest request, Func<IRestResponse, bool> condition)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int retries = 0;

            // Define retry policy
            var policy = Policy.HandleResult(condition).WaitAndRetry(5, (count) => TimeSpan.FromSeconds(10));

            // Execute the request with policy
            var val = policy.ExecuteAndCapture(() =>
            {
                Console.WriteLine($"{client.BaseUrl} - sent times: {++retries}");
                var response = client.Execute(request);
                Console.WriteLine($"Response: {response.StatusCode} {response.Content}");
                return response;
            });

            IRestResponse rr = val.Result;
            if (rr == null)
            {
                rr = new RestResponse();
                rr.Request = request;
                rr.ErrorException = val.FinalException;
            }

            stopwatch.Stop();
            Console.WriteLine($"The total time to get the expected response: {stopwatch.ElapsedMilliseconds} milliseconds.");
            return rr;
        }
    }
}
