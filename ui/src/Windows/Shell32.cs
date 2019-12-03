// <copyright file="Shell32.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Shell32.dll pinvoke library.
    /// </summary>
    public class Shell32
    {
        /// <summary>
        /// Specifies the action to be taken by this ShellNotifyIcon.
        /// </summary>
        [Flags]
        public enum NotifyIconMessage : int
        {
            /// <summary>
            /// Adds an icon to the status area.
            /// </summary>
            NimAdd = 0x00000000,

            /// <summary>
            /// Modifies an icon in the status area.
            /// </summary>
            NimModify = 0x00000001,

            /// <summary>
            /// Deletes an icon from the status area.
            /// </summary>
            NimDelete = 0x00000002,

            /// <summary>
            /// Returns focus to the taskbar notification area.
            /// </summary>
            NimSetFocus = 0x00000003,

            /// <summary>
            /// Instructs the notification area to behave according to the version number specified in the uVersion member of the structure pointed to by lpdata.
            /// </summary>
            NimSetVersion = 0x00000004,
        }

        /// <summary>
        /// Flags that either indicate which of the other members of the structure contain valid data or provide additional information to the tooltip as to how it should display.
        /// </summary>
        [Flags]
        public enum NotifyIconFlags : int
        {
            /// <summary>
            /// The uCallbackMessage member is valid.
            /// </summary>
            NifMessage = 0x00000001,

            /// <summary>
            /// The hIcon member is valid.
            /// </summary>
            NifIcon = 0x00000002,

            /// <summary>
            /// The szTip member is valid.
            /// </summary>
            NifTip = 0x00000004,

            /// <summary>
            /// The dwState and dwStateMask members are valid.
            /// </summary>
            NifState = 0x00000008,

            /// <summary>
            /// Display a balloon notification. The szInfo, szInfoTitle, dwInfoFlags, and uTimeout members are valid.
            /// </summary>
            NifInfo = 0x000000010,

            /// <summary>
            /// The guidItem is valid.
            /// </summary>
            NifGuid = 0x000000020,

            /// <summary>
            /// If the balloon notification cannot be displayed immediately, discard it.
            /// </summary>
            NifRealtime = 0x00000040,

            /// <summary>
            /// Use the standard tooltip.
            /// </summary>
            NifShowTip = 0x00000080,
        }

        /// <summary>
        /// Contains information that the system needs to display notifications in the notification area.
        /// </summary>
        public enum NotifyIconInfoFlags : int
        {
            /// <summary>
            /// No icon.
            /// </summary>
            NiifNone = 0x00000000,

            /// <summary>
            /// An information icon.
            /// </summary>
            NiifInfo = 0x00000001,

            /// <summary>
            /// A warning icon.
            /// </summary>
            NiifWarning = 0x00000002,

            /// <summary>
            /// An error icon.
            /// </summary>
            NiifError = 0x00000003,

            /// <summary>
            /// Use the icon identified in hBalloonIcon as the notification balloon's title icon.
            /// </summary>
            NiifUser = 0x00000004,

            /// <summary>
            /// Do not play the associated sound. Applies only to notifications.
            /// </summary>
            NiifNoSound = 0x00000010,

            /// <summary>
            /// The large version of the icon should be used as the notification icon.
            /// </summary>
            NiifLargeIcon = 0x00000020,

            /// <summary>
            /// Do not display the balloon notification if the current user is in "quiet time", which is the first hour after a new user logs into his or her account for the first time.
            /// </summary>
            NiifRespectQuietTime = 0x00000080,

            /// <summary>
            /// Reserved.
            /// </summary>
            NiifIconMask = 0x0000000F,
        }

        /// <summary>
        /// Sends a message to the taskbar's status area.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="notifyIconData">Icon data associated with the message.</param>
        /// <returns>Returns TRUE if successful, or FALSE otherwise.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "Shell_NotifyIcon")]
        public static extern bool ShellNotifyIcon(NotifyIconMessage message, Shell32Structures.NotifyIconData notifyIconData);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">Icon handle to destroy.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);
    }
}
