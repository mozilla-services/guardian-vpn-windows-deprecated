// <copyright file="Login.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Handles login into FxA accounts.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Gets the unique login urls for the user's signin attempt.
        /// </summary>
        /// <returns>A <see cref="JSONStructures.FxALoginURLs"/> object.</returns>
        public JSONStructures.FxALoginURLs GetLoginURLs()
        {
            var api = new ApiRequest(string.Empty, "/vpn/login", RestSharp.Method.POST);
            JSONStructures.FxALoginURLs loginURLs;

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-login-url-retrieval-error"), ErrorHandling.UserFacingErrorType.None, ErrorHandling.LogLevel.Error);
                return null;
            }

            try
            {
                loginURLs = JsonConvert.DeserializeObject<JSONStructures.FxALoginURLs>(response.Content);
                return loginURLs;
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }

            return null;
        }

        /// <summary>
        /// Queries the verification URL to check if the user has logged in.
        /// </summary>
        /// <param name="tokenURL">Login verification URL.</param>
        /// <returns>Returns the response content if successful, otherwise returns null.</returns>
        public string QueryRawLoginState(string tokenURL)
        {
            var api = new ApiRequest(string.Empty, tokenURL, RestSharp.Method.GET);

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorHandling.ErrorHandler.Handle("User has not logged in yet", ErrorHandling.LogLevel.Debug);
                return null;
            }

            return response.Content;
        }

        /// <summary>
        /// Deserializes the FxA login response from JSON.
        /// </summary>
        /// <param name="jsonContents">FxA login response.</param>
        /// <returns>A <see cref="JSONStructures.FxALogin"/> object.</returns>
        public JSONStructures.FxALogin ParseLoginState(string jsonContents)
        {
            try
            {
                var loginData = JsonConvert.DeserializeObject<JSONStructures.FxALogin>(jsonContents);
                return loginData;
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }

            return null;
        }

        /// <summary>
        /// Navigates to URL in the user's default browser.
        /// </summary>
        /// <param name="uri">URL to navigate to.</param>
        public void OpenBrowser(string uri)
        {
            // Validate URL first
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri parsedUri))
            {
                return;
            }

            if (parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps)
            {
                Process.Start(uri);
            }
        }

        /// <summary>
        /// Initiate the login attempt.
        /// </summary>
        public void StartLogin()
        {
            try
            {
                var loginURLs = GetLoginURLs();
                if (loginURLs == null)
                {
                    return;
                }

                var pollInterval = loginURLs.PollInterval % 31; // Max 30 seconds, no more
                Manager.Account.LoginState = FxA.LoginState.LoggingIn;
                StartQueryLoginThread(loginURLs.VerificationUrl, loginURLs.PollInterval, loginURLs.ExpiresOn);

                // Launch a browser
                OpenBrowser(loginURLs.LoginUrl);

                // Navigate to verification page
                UI.MainWindow mainWindow = (UI.MainWindow)Application.Current.MainWindow;
                mainWindow.NavigateToView(new FirefoxPrivateNetwork.UI.VerifyAccountView(loginURLs.LoginUrl), UI.MainWindow.SlideDirection.Left);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-add-device-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowWarning, ErrorHandling.LogLevel.Error);
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
            }
        }

        /// <summary>
        /// Intiates the login verification thread.
        /// </summary>
        /// <param name="queryUri">Login verification URL.</param>
        /// <param name="timeoutSeconds">Timeout (secs) between verification query attempts.</param>
        /// <param name="expiresAt">Expiration date of the verification URL.</param>
        public void StartQueryLoginThread(string queryUri, int timeoutSeconds, DateTime expiresAt)
        {
            var loginThread = new Thread(() => QueryLoginThread(queryUri, timeoutSeconds, expiresAt))
            {
                IsBackground = true,
            };
            loginThread.Start();
        }

        /// <summary>
        /// Polls a verification URL periodically to confirm if the user has logged in.
        /// </summary>
        /// <param name="queryUri">Login verification URL.</param>
        /// <param name="timeoutSeconds">Timeout (secs) between verification query attempts.</param>
        /// <param name="expiresAt">Expiration date of the verification URL.</param>
        private void QueryLoginThread(string queryUri, int timeoutSeconds, DateTime expiresAt)
        {
            while (Manager.Account.LoginState == FxA.LoginState.LoggingIn && DateTime.Compare(DateTime.UtcNow, expiresAt) < 0)
            {
                try
                {
                    var queryRawData = QueryRawLoginState(queryUri);

                    if (queryRawData == null)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(timeoutSeconds));
                        continue;
                    }

                    var queryData = ParseLoginState(queryRawData);

                    if (queryData.User == null || queryData.User.Subscriptions == null || queryData.User.Subscriptions.Vpn == null)
                    {
                        Manager.Account.LoginState = LoginState.LoggedOut;
                        break;
                    }

                    if (!queryData.User.Subscriptions.Vpn.Active)
                    {
                        Manager.Account.LoginState = LoginState.LoggedOut;
                        break;
                    }

                    var processLoginResult = Manager.Account.ProcessLogin(queryRawData);
                    Manager.Account.LoginState = LoginState.LoggedIn;
                    Manager.StartUIUpdaters();

                    var maxDevicesReached = !processLoginResult && Manager.Account.Config.FxALogin.User.Devices.Count() >= Manager.Account.Config.FxALogin.User.MaxDevices;

                    Cache.FxAServerList.RetrieveRemoteServerList();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var owner = Application.Current.MainWindow;
                        if (owner != null)
                        {
                            if (!Manager.MustUpdate)
                            {
                                if (maxDevicesReached)
                                {
                                    ((UI.MainWindow)owner).NavigateToView(new UI.DevicesView(deviceLimitReached: true, fxaJson: queryRawData), UI.MainWindow.SlideDirection.Left);
                                }
                                else
                                {
                                    ((UI.MainWindow)owner).NavigateToView(new UI.QuickAccessView(), UI.MainWindow.SlideDirection.Left);
                                }
                            }

                            ((UI.MainWindow)owner).Activate();
                        }
                    });
                }
                catch (Exception e)
                {
                    ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
