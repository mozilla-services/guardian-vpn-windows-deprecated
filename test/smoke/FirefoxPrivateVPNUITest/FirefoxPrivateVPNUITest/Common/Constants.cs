// <copyright file="Constants.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace FirefoxPrivateVPNUITest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The collection of constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Terms of service URL.
        /// </summary>
        public static readonly string TermsOfServiceUrl = $"https://www.mozilla.org/en-US/about/legal/terms/firefox-private-network/";

        /// <summary>
        /// Privacy Policy URL.
        /// </summary>
        public static readonly string PrivacyPolicyUrl = $"https://www.mozilla.org/en-US/privacy/firefox-private-network/";

        /// <summary>
        /// Privacy Notice.
        /// </summary>
        public static readonly string PrivacyNotice = @"Thank you for debugging the Firefox Private Network VPN client!
    
This utility will export a ZIP file to a directory of your choosing. This file will contain the following:
    
- A list of your running processes
- A list of your devices and device drivers
- Information about your network interfaces
- Your computer hardware information
    
Along with the VPN tunnel log, the currently available list of VPN servers will also be included.
Your Firefox account information and any of your VPN credentials will not be exported.
    
Do you wish to proceed?";

        /// <summary>
        /// Contact us url.
        /// </summary>
        public static readonly string ContactUsUrl = $"https://accounts.firefox.com/";

        /// <summary>
        /// Support url.
        /// </summary>
        public static readonly string SupportUrl = $"https://support.mozilla.org/en-US/products/firefox-private-network-vpn";

        /// <summary>
        /// Support url.
        /// </summary>
        public static readonly string FeedbackUrl = $"https://www.surveygizmo.com";

        /// <summary>
        /// Manage Account url.
        /// </summary>
        public static readonly string ManageAccountUrl = $"https://accounts.stage.mozaws.net";

        /// <summary>
        /// Am I mullvad connected API.
        /// </summary>
        public static readonly string AmIMullvadConnectedAPI = "https://am.i.mullvad.net/connected";

         /// <summary>
        /// Am I mullvad city API.
        /// </summary>
        public static readonly string AmIMullvadCityAPI = "https://am.i.mullvad.net/city";
    }
}
