// <copyright file="DebugLogger.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;

namespace FirefoxPrivateNetwork.ErrorHandling
{
    /// <summary>
    /// Logger helper class, used for logging object contents to a file.
    /// </summary>
    internal class DebugLogger
    {
        /// <summary>
        /// Logs the contents of any object by passing its ToString() value to the ErrorHandler class for further processing.
        /// </summary>
        /// <param name="debugMessages">Object to extract a string from and log.</param>
        public static void LogDebugMsg(params object[] debugMessages)
        {
            var debugMessageStrings = new List<string>();
            foreach (var dm in debugMessages)
            {
                if (dm == null)
                {
                    debugMessageStrings.Add("<null>");
                    continue;
                }

                try
                {
                    debugMessageStrings.Add(dm.ToString());
                }
                catch (Exception)
                {
                    debugMessageStrings.Add("-");
                }
            }

            ErrorHandler.WriteToLog(string.Join(" ", debugMessageStrings.ToArray()), LogLevel.Debug);
        }
    }
}
