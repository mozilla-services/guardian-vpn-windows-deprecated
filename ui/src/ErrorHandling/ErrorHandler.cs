// <copyright file="ErrorHandler.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FirefoxPrivateNetwork.ErrorHandling
{
    /// <summary>
    /// Severity of the error/notice being presented to the user.
    /// </summary>
    public enum UserFacingSeverity
    {
        /// <summary>
        /// Hide error from user.
        /// </summary>
        Hide,

        /// <summary>
        /// Shows a notice to the user.
        /// </summary>
        ShowNotice,

        /// <summary>
        /// Shows a warning to the user.
        /// </summary>
        ShowWarning,

        /// <summary>
        /// Shows an error to the user.
        /// </summary>
        ShowError,
    }

    /// <summary>
    /// Type of error display medium for the user.
    /// </summary>
    public enum UserFacingErrorType
    {
        /// <summary>
        /// No message shown.
        /// </summary>
        None,

        /// <summary>
        /// Internal app toast message shown.
        /// </summary>
        Toast,

        /// <summary>
        /// Windows tray toast shown.
        /// </summary>
        WindowsToast,

        /// <summary>
        /// Modal dialog shown.
        /// </summary>
        ModalMessage,
    }

    /// <summary>
    /// Logging level indicators. Used in tandem with the logging infrastructure to determine what needs to be logged.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log only Info messages.
        /// </summary>
        Info,

        /// <summary>
        /// Log only Debug messages.
        /// </summary>
        Debug,

        /// <summary>
        /// Log only Error messages.
        /// </summary>
        Error,

        /// <summary>
        /// Don't log anything.
        /// </summary>
        None,
    }

    /// <summary>
    /// Error handler helper class.
    /// Handles logging and exception handling.
    /// </summary>
    internal class ErrorHandler
    {
        /// <summary>
        /// Ringlogger log file instance.
        /// </summary>
        public static readonly WireGuard.Ringlogger Ringlogger = new WireGuard.Ringlogger(ProductConstants.FirefoxPrivateNetworkLogFile, "FPN");

        /// <summary>
        /// Handle an exception by writing to the log.
        /// </summary>
        /// <param name="e">Exception to log.</param>
        public static void Handle(Exception e)
        {
            WriteToLog(e.Message, LogLevel.Debug);
        }

        /// <summary>
        /// Log a message to the log file.
        /// </summary>
        /// <param name="msg">Message to log to the log file.</param>
        public static void Handle(string msg)
        {
            WriteToLog(msg, LogLevel.Debug);
        }

        /// <summary>
        /// Write a user facing message to the log and potentially display it.
        /// </summary>
        /// <param name="msg">Message to log and potentially display.</param>
        /// <param name="errorType">Type of user facing error.</param>
        public static void Handle(UserFacingMessage msg, UserFacingErrorType errorType)
        {
            WriteToLog(msg.Text, LogLevel.Debug);
        }

        /// <summary>
        /// Handle an exception by writing to the log based on the supplied log level.
        /// </summary>
        /// <param name="e">Exception to potentailly log.</param>
        /// <param name="logLevel">Desired level of logging.</param>
        public static void Handle(Exception e, LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
            {
                WriteToLog(e.ToString(), logLevel);
            }
            else
            {
                WriteToLog(e.Message, logLevel);
            }
        }

        /// <summary>
        /// Write to the log based on the supplied log level.
        /// </summary>
        /// <param name="msg">Message to potentailly log.</param>
        /// <param name="logLevel">Desired level of logging.</param>
        public static void Handle(string msg, LogLevel logLevel)
        {
            WriteToLog(msg, logLevel);
        }

        /// <summary>
        /// Handle a user facing message by potentially writing to the log and/or displaying it to the user.
        /// </summary>
        /// <param name="msg">User facing message to potentially display and/or log.</param>
        /// <param name="errorType">User facing error type.</param>
        /// <param name="logLevel">Desired level of logging.</param>
        public static void Handle(UserFacingMessage msg, UserFacingErrorType errorType, LogLevel logLevel)
        {
            WriteToLog(msg.Text, logLevel);
        }

        /// <summary>
        /// Handle a user facing message by potentially writing to the log and/or displaying it to the user.
        /// </summary>
        /// <param name="msg">User facing message to potentially display and/or log.</param>
        /// <param name="errorType">User facing error type.</param>
        /// <param name="severity">Type of severity presented to the user when displaying a message.</param>
        /// <param name="logLevel">Desired level of logging.</param>
        public static void Handle(UserFacingMessage msg, UserFacingErrorType errorType, UserFacingSeverity severity, LogLevel logLevel)
        {
            WriteToLog(msg.Text, logLevel);
        }

        /// <summary>
        /// Write a message to th elog based on the supplied log level.
        /// </summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="logLevel">Desired level of logging.</param>
        public static void WriteToLog(string msg, LogLevel logLevel)
        {
            if (logLevel >= ProductConstants.LogLevel)
            {
                Ringlogger.Write(msg);
            }
        }

        /// <summary>
        /// Show a modal message to the user.
        /// </summary>
        /// <param name="msg">User facing message to display.</param>
        /// <param name="severity">Type of severity presented to the user when displaying a message.</param>
        private static void HandleUserFacingModalMessage(UserFacingMessage msg, UserFacingSeverity severity)
        {
            switch (severity)
            {
                case UserFacingSeverity.ShowError:
                    MessageBox.Show(msg.Text, ProductConstants.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                case UserFacingSeverity.ShowNotice:
                    MessageBox.Show(msg.Text, ProductConstants.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case UserFacingSeverity.ShowWarning:
                    MessageBox.Show(msg.Text, ProductConstants.ProductName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                case UserFacingSeverity.Hide:
                default:

                    break;
            }
        }
    }
}
