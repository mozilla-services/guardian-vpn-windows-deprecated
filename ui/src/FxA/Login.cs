// <copyright file="Login.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Newtonsoft.Json;
using RestSharp.Extensions;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Handles login into FxA accounts.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        /// <param name="handler">Handler for the login result event.</param>
        public Login(LoginResultHandler handler = null)
        {
            LoginResultEvent = handler;
        }

        /// <summary>
        /// Handler for the result of a login session.
        /// </summary>
        /// <param name="sender">Sender of the login result.</param>
        /// <param name="session">Login object that is associated with the login result.</param>
        /// <param name="state">The result of the login session.</param>
        public delegate void LoginResultHandler(object sender, Login session, LoginState state);

        private event LoginResultHandler LoginResultEvent;

        /// <summary>
        /// Gets or sets the API version for making PKCE auth requests and handling responses.
        /// </summary>
        public static string ApiVersion { get; set; } = "/api/v2";

        /// <summary>
        /// ref http://stackoverflow.com/a/3978040.
        /// </summary>
        /// <returns>Random open port number.</returns>
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);

            listener.Start();

            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            listener.Stop();

            return port;
        }

        /// <summary>
        /// Navigates to URL in the user's default browser.
        /// </summary>
        public async void OpenBrowser()
        {
            // Generate code_verifier
            string codeVerifier = GetUniqueToken(44);

            // Get a random, unused port number to use in the PKCE request
            int portNumber = GetRandomUnusedPort();

            // Generate the code_challenge by getting the SHA256 hash and encoding it to send in a request
            // Switched the code_challenge_method to plain since there were intermittent issues with the SHA256 method returning the user data each time.
            string codeChallenge = codeVerifier;

            // The codeChallenge is there to handle the PKCE request to the server
            string loginURL = $"{ProductConstants.BaseUrl}{ApiVersion}/vpn/login/windows?code_challenge={codeChallenge}&code_challenge_method=plain&port={portNumber}";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, portNumber);

            // Open browser with the URL
            Process.Start(loginURL);

            // Creates an HttpListener to listen for requests on that redirect URI.
            HttpListener http = new HttpListener();

            // Make sure HTTP listener is listening to the localhost:{portNumber} URI
            http.Prefixes.Add(redirectURI);
            http.Start();

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Sends an HTTP response to the browser.
            var response = context.Response;

            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://www.mozilla.org/'></head><body>Please return to the app.</body></html>");

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            Stream responseOutput = response.OutputStream;

            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
            });

            // extracts the code
            string code = context.Request.QueryString.Get("code");

            // Starts the code exchange at the Token Endpoint.
            VerifyUserLogin(codeVerifier, code);
        }

        /// <summary>
        /// Initiate the login attempt.
        /// </summary>
        /// <returns>Whether the login process is started succefully.</returns>
        public bool StartLogin()
        {
            try
            {
                Manager.Account.LoginState = LoginState.LoggingIn;

                // Launch a browser
                OpenBrowser();
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-add-device-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowWarning, ErrorHandling.LogLevel.Error);
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets verification code from mozilla-vpn:// redirect after the user logs in through the browser.
        /// </summary>
        /// <param name="codeVerifier">The code sent in the PKCE auth request.</param>
        /// <param name="code">Verification code from the localhost redirect after logging in.</param>
        public void VerifyUserLogin(string codeVerifier, string code)
        {
            // Make the POST request to the verify endpoint
            ApiRequest api = new ApiRequest(string.Empty, $"{ProductConstants.BaseUrl}{ApiVersion}/vpn/login/verify", RestSharp.Method.POST);

            // Create a new dictionary to hold the code and code_verifier.
            Dictionary<string, string> postBody = new Dictionary<string, string>();
            postBody.Add("code", code);
            postBody.Add("code_verifier", codeVerifier);
            api.AddPostBody(postBody);

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-login-url-retrieval-error"), ErrorHandling.UserFacingErrorType.None, ErrorHandling.LogLevel.Error);
                return;
            }

            // Check login credentials, add the session info to the settings, and add a new device
            bool processLoginResult = Manager.Account.ProcessLogin(response.Content);
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
                            ((UI.MainWindow)owner).NavigateToView(new UI.DevicesView(deviceLimitReached: true, fxaJson: response.Content), UI.MainWindow.SlideDirection.Left);
                        }
                        else
                        {
                            ((UI.MainWindow)owner).NavigateToView(new UI.OnboardingView5(), UI.MainWindow.SlideDirection.Left);
                        }
                    }

                    ((UI.MainWindow)owner).Show();
                    ((UI.MainWindow)owner).WindowState = WindowState.Normal;
                    ((UI.MainWindow)owner).Activate();
                }
            });

            LoginResultEvent?.Invoke(this, this, Manager.Account.LoginState);
        }

        /// <summary>
        /// Generates a Base64 encoded string based on a SHA256 hash with the string it's given.
        /// </summary>
        /// <param name="text">The text that needs to be converted to a SHA256 hash.</param>
        /// <returns>A Base64 encoded SHA256 hash.</returns>
        private static string GetHashSha256(string text)
        {
            SHA256Managed hashTool = new SHA256Managed();

            byte[] phraseAsByte = Encoding.UTF8.GetBytes(string.Concat(text));
            byte[] encryptedBytes = hashTool.ComputeHash(phraseAsByte);

            hashTool.Clear();

            string value = Convert.ToBase64String(encryptedBytes);

            return value;
        }

        /// <summary>
        /// Generates a random cryptographic string to use in a PKCE auth request and response.
        /// </summary>
        /// <param name="length">The number of characters that should be in the string.</param>
        /// <param name="chars">The acceptable characters that can be in the string.</param>
        /// <returns>A cryptographic string.</returns>
        private static string GetUniqueToken(int length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_")
        {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];

                // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer.
                byte[] buffer = null;

                // Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }
    }
}
