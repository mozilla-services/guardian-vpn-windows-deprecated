// <copyright file="Utils.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;
    using Polly;
    using RestSharp;

    /// <summary>
    /// Here are some utilities.
    /// </summary>
    public class Utils
    {
        private static readonly int RetryTimes = 10;
        private static readonly int RetryDelayInSeconds = 10;
        private static readonly int RetrySleepInMilliseconds = 200;

        /// <summary>
        /// Random select an index from integers based on the condition.
        /// </summary>
        /// <param name="range">A range of integers.</param>
        /// <param name="condition">A condition method.</param>
        /// <returns>A random index.</returns>
        public static int RandomSelectIndex(IEnumerable<int> range, Func<int, bool> condition)
        {
            // If the range has no element.
            if (range.Count() <= 0)
            {
                return -1;
            }

            var result = range.Where(condition);

            // If the filtered result has no element
            if (result.Count() <= 0)
            {
                return -1;
            }

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
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="findMethod">The method used to find element.</param>
        /// <param name="selector">The selector used in the findMethod.</param>
        /// <param name="timeOut">Time out in milliseconds. Default is 60000 milliseconds.</param>
        /// <returns>Generic Element.</returns>
        public static T WaitUntilFindElement<T>(Func<string, T> findMethod, string selector, double timeOut = 60000)
        {
            T element = default(T);
            Utils.WaitUntil(ref element, findMethod, selector, (ele) => ele != null, timeOut);
            return element;
        }

        /// <summary>
        /// Wait for {timeOut} milliseconds until find elements.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="findMethod">The method used to find element.</param>
        /// <param name="selector">The selector used in the findMethod.</param>
        /// <param name="timeOut">Time out in milliseconds. Default is 60000 milliseconds.</param>
        /// <returns>Read Only collection of T.</returns>
        public static ReadOnlyCollection<T> WaitUntilFindElements<T>(Func<string, ReadOnlyCollection<T>> findMethod, string selector, double timeOut = 60000)
        {
            ReadOnlyCollection<T> elements = null;
            Utils.WaitUntil(ref elements, findMethod, selector, (ele) => ele != null && ele.Count > 0, timeOut);
            return elements;
        }

        /// <summary>
        /// Generic method to implement wait until.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="result">The reference to the result.</param>
        /// <param name="func">A function which takes string as parameter and return T.</param>
        /// <param name="param">The parameter passed to func.</param>
        /// <param name="condition">A function which takes result as parameter and return bool.</param>
        /// <param name="timeOut">The time out, default 60000 ms.</param>
        public static void WaitUntil<T>(ref T result, Func<string, T> func, string param, Func<T, bool> condition, double timeOut = 60000)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            bool retry = true;
            int count = 0;
            while (retry && time.ElapsedMilliseconds <= timeOut)
            {
                try
                {
                    Console.WriteLine($"Wait for {func.Method.Name} with {param}: {++count}");
                    result = func(param);

                    if (condition(result))
                    {
                        retry = false;
                        time.Stop();
                    }
                    else
                    {
                        retry = true;
                        Thread.Sleep(TimeSpan.FromMilliseconds(RetrySleepInMilliseconds));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error message for {func.Method.Name} with {param}: {ex.Message}");
                    retry = true;
                    Thread.Sleep(TimeSpan.FromMilliseconds(RetrySleepInMilliseconds));
                }
            }

            if (time.ElapsedMilliseconds > timeOut)
            {
                time.Stop();
            }

            Console.WriteLine($"Total waiting time for {func.Method.Name} with {param}: {time.ElapsedMilliseconds} milliseconds.");
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
        /// Get verification code from email subject.
        /// </summary>
        /// <param name="user">The email user name.</param>
        /// <returns>The verification code.</returns>
        public static string GetVerificationCode(string user)
        {
            var client = new RestClient($"{Constants.RestMailAPI}/{user}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            Func<IRestResponse, bool> condition = (res) =>
            {
                return res.StatusCode != HttpStatusCode.OK || res.Content == "[]";
            };
            IRestResponse response = RetryExecute(client, request, condition);
            dynamic json = Newtonsoft.Json.Linq.JArray.Parse(response.Content);
            string subject = Convert.ToString(json[0].subject);
            string verificationCode = subject.Split(':')[1].Trim();
            return verificationCode;
        }

        /// <summary>
        /// Delete user from rest mail.
        /// </summary>
        /// <param name="user">The email user name.</param>
        /// <returns>Succeed or not.</returns>
        public static bool DeleteUserFromRestMail(string user)
        {
            var client = new RestClient($"{Constants.RestMailAPI}/{user}");
            client.Timeout = -1;
            var request = new RestRequest(Method.DELETE);
            Func<IRestResponse, bool> condition = (res) =>
            {
                return res.StatusCode != HttpStatusCode.OK;
            };
            IRestResponse response = RetryExecute(client, request, condition);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Re-arrange the position and size of vpn client and browser windows to prevent overlap.
        /// </summary>
        /// <param name="vpnClient">VPN client session.</param>
        /// <param name="browser">Browser session.</param>
        public static void RearrangeWindows(BaseSession vpnClient, BaseSession browser)
        {
            vpnClient.SetWindowPosition(0, 0);
            var vpnClientPosition = vpnClient.Session.Manage().Window.Position;
            var vpnClientSize = vpnClient.Session.Manage().Window.Size;
            browser.SetWindowPosition(vpnClientPosition.X + vpnClientSize.Width, 0);
            browser.SetWindowSize(600, 650);
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
            var policy = Policy.HandleResult(condition).WaitAndRetry(RetryTimes, (count) => TimeSpan.FromSeconds(RetryDelayInSeconds));

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
