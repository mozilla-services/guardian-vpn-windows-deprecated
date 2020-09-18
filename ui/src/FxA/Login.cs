// <copyright file="Login.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp.Extensions;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Handles login into FxA accounts.
    /// </summary>
    public class Login
    {
        private static string codeVerifierPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        private static string codeVerifierFile = "moz_gen_cvf.txt";

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
        /// Gets the unique login urls for the user's signin attempt.
        /// </summary>
        /// <returns>A <see cref="JSONStructures.FxALoginURLs"/> object.</returns>
        public JSONStructures.FxALoginURLs GetLoginURL()
        {
            // Generate code_verifier
            string codeVerifier = GetUniqueToken(44);

            // Save the code_verifier for use in the verify request
            File.WriteAllText(Path.Combine(codeVerifierPath, codeVerifierFile), codeVerifier);

            // Generate the code_challenge by getting the SHA256 hash and encoding it to send in a request
            // Switched the code_challenge_method to plain since there were intermittent issues with the SHA256 method returning the user data each time.
            string codeChallenge = codeVerifier;

            // Make new instance of object that hold the URLs needed for a user to login
            JSONStructures.FxALoginURLs loginURLs = new JSONStructures.FxALoginURLs();

            try
            {
                // The codeChallenge is there to handle the PKCE request to the server
                loginURLs.LoginUrl = $"{ProductConstants.BaseUrl}{ApiVersion}/vpn/login/windows?code_challenge={codeChallenge}&code_challenge_method=plain";

                return loginURLs;
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
        /// <returns>Whether the login process is started succefully.</returns>
        public bool StartLogin()
        {
            try
            {
                // Get the login and verify URLs
                JSONStructures.FxALoginURLs loginURL = GetLoginURL();

                if (loginURL == null)
                {
                    return false;
                }

                // Create the GET request for PKCE auth
                ApiRequest api = new ApiRequest(string.Empty, loginURL.LoginUrl, RestSharp.Method.GET);

                // Execute the request
                var response = api.SendRequest();

                if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-login-url-retrieval-error"), ErrorHandling.UserFacingErrorType.None, ErrorHandling.LogLevel.Error);
                    return false;
                }

                Manager.Account.LoginState = LoginState.LoggingIn;

                // Launch a browser
                OpenBrowser(loginURL.LoginUrl);
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
        /// <param name="code">Verification code from the custom URL protocol redirect after logging in.</param>
        public void VerifyUserLogin(string code)
        {
            // Make the POST request to the verify endpoint
            ApiRequest api = new ApiRequest(string.Empty, $"{ProductConstants.BaseUrl}{ApiVersion}/vpn/login/verify", RestSharp.Method.POST);

            // Get code_verifier that was generated in the login request
            string code_verifier = File.ReadAllText(Path.Combine(codeVerifierPath, codeVerifierFile));

            // Create a new dictionary to hold the code and code_verifier.
            Dictionary<string, string> postBody = new Dictionary<string, string>();
            postBody.Add("code", code);
            postBody.Add("code_verifier", code_verifier);
            api.AddPostBody(postBody);

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-login-url-retrieval-error"), ErrorHandling.UserFacingErrorType.None, ErrorHandling.LogLevel.Error);
                return;
            }

            // Check login credentials, add the session info to the settings, and add a new device
            Manager.Account.ProcessLogin(response.Content);
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
