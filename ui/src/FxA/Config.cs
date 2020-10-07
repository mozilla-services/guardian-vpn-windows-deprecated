// <copyright file="Config.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.IO;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// User's FxA login session configuration.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="fxaLoginJson">Raw JSON data to parse and create a new object from.</param>
        public Config(string fxaLoginJson)
        {
            LoadFxALoginFromString(fxaLoginJson);
        }

        /// <summary>
        /// Gets or sets the FxA Login JSON structure containing the user token, public key, etc.
        /// </summary>
        public JSONStructures.FxALogin FxALogin { get; set; } = new JSONStructures.FxALogin();

        /// <summary>
        /// Load the user's FxA login session from a JSON string.
        /// </summary>
        /// <param name="fxaLoginJson">Raw JSON data to parse.</param>
        public void LoadFxALoginFromString(string fxaLoginJson)
        {
            FxALogin = JsonConvert.DeserializeObject<JSONStructures.FxALogin>(fxaLoginJson);
        }

        /// <summary>
        /// Load the FxA token from settings.
        /// </summary>
        /// <returns>Returns true if the load succeeded.</returns>
        public bool LoadFxAToken()
        {
            if (string.IsNullOrEmpty(Manager.Settings.FxA.Token) || string.IsNullOrEmpty(Manager.Settings.FxA.PublicKey))
            {
                return false;
            }

            FxALogin.Token = Manager.Settings.FxA.Token;
            FxALogin.PublicKey = Manager.Settings.FxA.PublicKey;

            return true;
        }

        /// <summary>
        /// Load the login session's FxA user data from a file.
        /// </summary>
        /// <param name="fxaUserFile">Saved raw JSON data in a file from which the FxA user is to be created.</param>
        /// <returns>Returns true if the load succeeded.</returns>
        public bool LoadFxAUserFromFile(string fxaUserFile)
        {
            if (!File.Exists(fxaUserFile))
            {
                return false;
            }

            try
            {
                string content = File.ReadAllText(fxaUserFile);
                FxALogin.User = DeserializeUserFileContent(content);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        private JSONStructures.User DeserializeUserFileContent(string content)
        {
            string decryptResult = Decrypt(content);

            JSONStructures.User user;
            if (decryptResult == null)
            {
                user = JsonConvert.DeserializeObject<JSONStructures.User>(content);
            }
            else
            {
                user = JsonConvert.DeserializeObject<JSONStructures.User>(decryptResult);
            }

            return user;
        }

        /// <summary>
        /// Save the login session's FxA token to settings.
        /// </summary>
        public void SaveFxAToken()
        {
            var fxaSettings = Manager.Settings.FxA;
            fxaSettings.Token = FxALogin.Token;
            fxaSettings.PublicKey = FxALogin.PublicKey;
            Manager.Settings.FxA = fxaSettings;
        }

        /// <summary>
        /// Remove the FxA token from settings.
        /// </summary>
        public void RemoveFxAToken()
        {
            var fxaSettings = Manager.Settings.FxA;
            fxaSettings.Token = string.Empty;
            fxaSettings.PublicKey = string.Empty;
            Manager.Settings.FxA = fxaSettings;
        }

        /// <summary>
        /// Save the login session's FxA user data to a file.
        /// </summary>
        /// <param name="filename">Name of the file that the FxA user details are stored.</param>
        public void WriteFxAUserToFile(string filename)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filename);
            Directory.CreateDirectory(file.Directory.FullName);
            string content = JsonConvert.SerializeObject(FxALogin.User);
            string encrypted = Encrypt(content);
            if (encrypted == null)
            {
                File.WriteAllText(filename, content);
            }
            else
            {
                File.WriteAllText(filename, encrypted);
            }
        }

        /// <summary>
        /// Helper function that writes to a file.
        /// </summary>
        /// <param name="filename">Name of the file to write to.</param>
        /// <param name="data">Data to write to the file.</param>
        /// <returns>Success status of the file writing.</returns>
        public bool WriteToFile(string filename, string data)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filename);
                Directory.CreateDirectory(file.Directory.FullName);
                File.WriteAllText(filename, data);
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        private string Encrypt(string plainText)
        {
            try
             {
                byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedByte = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedByte);
            }
            catch (FormatException e)
            {
                return null;
            }
            catch (CryptographicException e)
            {
                return null;
            }
            catch (NotSupportedException e)
            {
                return null;
            }
        }

        private string Decrypt(string encrypted)
        {
            try
            {
                byte[] encryptedByte = Convert.FromBase64String(encrypted);
                byte[] decryptedByte = ProtectedData.Unprotect(encryptedByte, null, DataProtectionScope.CurrentUser);
                return System.Text.Encoding.UTF8.GetString(decryptedByte);
            }
            catch (FormatException e)
            {
                return null;
            }
            catch (CryptographicException e)
            {
                return null;
            }
            catch (NotSupportedException e)
            {
                return null;
            }
        }
    }
}
