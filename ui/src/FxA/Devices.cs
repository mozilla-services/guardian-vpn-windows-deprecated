// <copyright file="Devices.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Represents a device associated with a FxA account.
    /// </summary>
    internal class Devices
    {
        /// <summary>
        /// Add a device to the user's FxA account through the FxA API.
        /// </summary>
        /// <param name="deviceName">Naming convention: machine name + platform + version.</param>
        /// <param name="publicKey">A unique key, part of a keypair generated using BCrypt algorithm.</param>
        /// <returns>Returns the user's device if successful, otherwise null.</returns>
        public JSONStructures.Device AddDevice(string deviceName, string publicKey)
        {
            var api = new ApiRequest(Manager.Account.Config.FxALogin.Token, "/vpn/device", RestSharp.Method.POST);
            api.AddPostBody(new Dictionary<string, string> { { "name", deviceName }, { "pubkey", publicKey } });

            // Execute the request
            var response = api.SendRequest();

            if (response == null || response.StatusCode < System.Net.HttpStatusCode.OK || response.StatusCode >= System.Net.HttpStatusCode.BadRequest)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-add-device-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowWarning, ErrorHandling.LogLevel.Error);
                return null;
            }

            try
            {
                var newDevice = JsonConvert.DeserializeObject<JSONStructures.Device>(response.Content);
                Manager.Account.Config.FxALogin.User.Devices.Add(newDevice);
                Manager.Account.Config.WriteFxAUserToFile(ProductConstants.FxAUserFile);

                return newDevice;
            }
            catch (Exception)
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-add-device-error"), ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowWarning, ErrorHandling.LogLevel.Error);
            }

            return null;
        }

        /// <summary>
        /// Remove a device by its public key through the FxA API.
        /// </summary>
        /// <param name="publicKey">Public key of the device to be removed.</param>
        /// <param name="safeRemove">Flag that indicates whether to check if the user's current device is being removed.</param>
        /// <param name="silent">Flag that indicates if an in-app toast.</param>
        /// <returns>Success status of the device removal.</returns>
        public bool RemoveDevice(string publicKey, bool safeRemove = false, bool silent = false)
        {
            // SafeRemove indicates that the device cannot be removed if it is the user's current device
            if (safeRemove)
            {
                if (publicKey == Manager.Account.Config.FxALogin.PublicKey)
                {
                    ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-remove-current-device-error"), silent ? ErrorHandling.UserFacingErrorType.None : ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.LogLevel.Error);
                    return false;
                }
            }

            // Url encoding the device public key
            var sanitizedDevicePublicKey = System.Web.HttpUtility.UrlEncode(publicKey);
            var api = new ApiRequest(Manager.Account.Config.FxALogin.Token, "/vpn/device/" + sanitizedDevicePublicKey, RestSharp.Method.DELETE);

            // Execute the request`
            var response = api.SendRequest();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Manager.Account.Config.FxALogin.User.Devices.RemoveAll(d => d.PublicKey == publicKey);
                Manager.Account.Config.WriteFxAUserToFile(ProductConstants.FxAUserFile);

                Manager.AccountInfoUpdater.RefreshDeviceList();
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-remove-device-success"), silent ? ErrorHandling.UserFacingErrorType.None : ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowNotice, ErrorHandling.LogLevel.Info);

                return true;
            }
            else
            {
                ErrorHandling.ErrorHandler.Handle(new ErrorHandling.UserFacingMessage("toast-remove-device-error"), silent ? ErrorHandling.UserFacingErrorType.None : ErrorHandling.UserFacingErrorType.Toast, ErrorHandling.UserFacingSeverity.ShowError, ErrorHandling.LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Async task to remove an account device by its public key through the FxA API.
        /// </summary>
        /// <param name="publicKey">Public key of the device to delete.</param>
        /// <param name="safeRemove">Optional: Indicates the current user's device cannot be deleted when set to true.</param>
        /// <param name="silent">Optional: Indicates no Messageboxes will be shown upon successful/unsuccessful device removal when set to true.</param>
        /// <returns>The remove device task.</returns>
        public Task<bool> RemoveDeviceTask(string publicKey, bool safeRemove = false, bool silent = false)
        {
            var removeDeviceTask = Task.Run(() =>
            {
                return RemoveDevice(publicKey, safeRemove, silent);
            });

            return removeDeviceTask;
        }
    }
}
