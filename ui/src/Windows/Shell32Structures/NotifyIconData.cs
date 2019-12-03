// <copyright file="NotifyIconData.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows.Shell32Structures
{
    /// <summary>
    /// Contains information that the system needs to display notifications in the notification area.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1401:FieldShouldBePrivate", Justification = "PInvoke structure")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class NotifyIconData
    {
        /// <summary>
        /// The size of this structure, in bytes.
        /// </summary>
        public int Size = Marshal.SizeOf(typeof(NotifyIconData));

        /// <summary>
        /// A handle to the window that receives notifications associated with an icon in the notification area.
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        /// The application-defined identifier of the taskbar icon.
        /// </summary>
        public int UId;

        /// <summary>
        /// Flags that either indicate which of the other members of the structure contain valid data or provide additional information to the tooltip as to how it should display.
        /// </summary>
        public int Flags;

        /// <summary>
        /// An application-defined message identifier. The system uses this identifier to send notification messages to the window identified in hWnd.
        /// </summary>
        public int CallbackMessage;

        /// <summary>
        /// A handle to the icon to be added, modified, or deleted.
        /// </summary>
        public IntPtr IconHandle;

        /// <summary>
        /// Tip text contents.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Tip;

        /// <summary>
        /// The state of the icon.
        /// </summary>
        public int State;

        /// <summary>
        /// A value that specifies which bits of the dwState member are retrieved or modified.
        /// </summary>
        public int StateMask;

        /// <summary>
        /// A null-terminated string that specifies the text to display in a balloon notification.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Info;

        /// <summary>
        /// Union with uTimeout (deprecated as of Windows Vista). Specifies which version of the Shell notification icon interface should be used.
        /// </summary>
        public int Timeout;

        /// <summary>
        /// A null-terminated string that specifies a title for a balloon notification.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string InfoTitle;

        /// <summary>
        /// Flags that can be set to modify the behavior and appearance of a balloon notification. The icon is placed to the left of the title.
        /// </summary>
        public int InfoFlags;

        /// <summary>
        /// A registered GUID that identifies the icon.
        /// </summary>
        public Guid GuidItem;

        /// <summary>
        ///  The handle of a customized notification icon provided by the application that should be used independently of the notification area icon.
        /// </summary>
        public IntPtr BalloonIconHandle;
    }
}
