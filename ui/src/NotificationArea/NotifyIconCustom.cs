// <copyright file="NotifyIconCustom.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.NotificationArea
{
    /// <summary>
    /// Type of icon to show in the system tray.
    /// </summary>
    public enum IconType
    {
        /// <summary>
        /// Disconnected icon.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connected icon.
        /// </summary>
        Connected,

        /// <summary>
        /// Unstable icon.
        /// </summary>
        Unstable,

        /// <summary>
        /// No signal icon.
        /// </summary>
        NoSignal,
    }

    /// <summary>
    /// Type of icon to show when displaying a toast.
    /// </summary>
    public enum ToastIconType
    {
        /// <summary>
        /// Disconnected icon.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connected icon.
        /// </summary>
        Connected,

        /// <summary>
        /// Switched icon.
        /// </summary>
        Switched,
    }

    /// <summary>
    /// Type of event handler for when a toast is clicked.
    /// </summary>
    public enum ToastClickEvent
    {
        /// <summary>
        /// Do nothing when the toast is clicked.
        /// </summary>
        None = Windows.User32.WmApp + 1,

        /// <summary>
        /// Send connect command to the tunnel when the toast is clicked.
        /// </summary>
        Connect,

        /// <summary>
        /// Send disconnect command to the tunnel when the toast is clicked.
        /// </summary>
        Disconnect,
    }

    /// <summary>
    /// Custom TrayIcon class for handling the displaying and interaction of the application's tray icon.
    /// </summary>
    public class NotifyIconCustom : IDisposable
    {
        private readonly IntPtr hWnd;
        private readonly int uId = 1;

        private readonly Dictionary<IconType, Icon> icons;
        private readonly Dictionary<IconType, IntPtr> iconHandles;

        private readonly Dictionary<ToastIconType, Icon> toastIcons;
        private readonly Dictionary<ToastIconType, IntPtr> toastIconHandles;

        private bool disposed = false;
        private bool addedToTray;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyIconCustom"/> class.
        /// </summary>
        /// <param name="windowHandle">hWnd window handle to use as a message handler.</param>
        /// <param name="iconTooltip">Default icon tooltip to show when a mouse hover occurs over the tray icon.</param>
        public NotifyIconCustom(IntPtr windowHandle, string iconTooltip)
        {
            hWnd = windowHandle;

            // Set up icons
            icons = new Dictionary<IconType, Icon>
            {
                { IconType.Disconnected, new Icon(Properties.Resources.TrayIconOff, 64, 64) },
                { IconType.Connected, new Icon(Properties.Resources.TrayIconOn, 64, 64) },
                { IconType.Unstable, new Icon(Properties.Resources.TrayIconUnstable, 64, 64) },
                { IconType.NoSignal, new Icon(Properties.Resources.TrayIconUnstable, 64, 64) },
            };

            iconHandles = new Dictionary<IconType, IntPtr>
            {
                { IconType.Disconnected, icons[IconType.Disconnected].ToBitmap().GetHicon() },
                { IconType.Connected, icons[IconType.Connected].ToBitmap().GetHicon() },
                { IconType.Unstable, icons[IconType.Unstable].ToBitmap().GetHicon() },
                { IconType.NoSignal, icons[IconType.NoSignal].ToBitmap().GetHicon() },
            };

            // Set up toast icons
            toastIcons = new Dictionary<ToastIconType, Icon>
            {
                { ToastIconType.Connected, new Icon(Properties.Resources.TrayToastOn, 64, 64) },
                { ToastIconType.Disconnected, new Icon(Properties.Resources.TrayToastOff, 64, 64) },
                { ToastIconType.Switched, new Icon(Properties.Resources.TrayToastOn, 64, 64) },
            };

            toastIconHandles = new Dictionary<ToastIconType, IntPtr>
            {
                { ToastIconType.Connected, toastIcons[ToastIconType.Connected].ToBitmap().GetHicon() },
                { ToastIconType.Disconnected, toastIcons[ToastIconType.Disconnected].ToBitmap().GetHicon() },
                { ToastIconType.Switched, toastIcons[ToastIconType.Switched].ToBitmap().GetHicon() },
            };

            UpdateIcon(IconType.Disconnected, iconTooltip, true);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NotifyIconCustom"/> class.
        /// </summary>
        ~NotifyIconCustom()
        {
            Dispose(false);
        }

        /// <summary>
        /// Update the tray icon with new data.
        /// </summary>
        /// <param name="icon">Type of icon to present.</param>
        /// <param name="iconTooltip">Icon tooltip text.</param>
        /// <param name="visible">Sets the visibility parameter of the tray icon.</param>
        public void UpdateIcon(IconType icon, string iconTooltip, bool visible = true)
        {
            var nIconData = new Windows.Shell32Structures.NotifyIconData
            {
                CallbackMessage = 2048,
                Flags = (int)Shell32.NotifyIconFlags.NifMessage | (int)Shell32.NotifyIconFlags.NifIcon | (int)Shell32.NotifyIconFlags.NifTip,
                Handle = hWnd,
                UId = uId,
                IconHandle = iconHandles[icon],
                Tip = iconTooltip,
            };

            if (visible)
            {
                if (!addedToTray)
                {
                    Shell32.ShellNotifyIcon(Shell32.NotifyIconMessage.NimAdd, nIconData);
                    addedToTray = true;
                }
                else
                {
                    Shell32.ShellNotifyIcon(Shell32.NotifyIconMessage.NimModify, nIconData);
                }
            }
            else
            {
                if (addedToTray)
                {
                    Shell32.ShellNotifyIcon(Shell32.NotifyIconMessage.NimDelete, nIconData);
                    addedToTray = false;
                }
            }
        }

        /// <summary>
        /// Shows a balloon tip/toast in the system tray.
        /// </summary>
        /// <param name="timeout">Number of miliseconds after which the tip is hidden.</param>
        /// <param name="tipTitle">Title of the balloon/toast.</param>
        /// <param name="tipText">Contents of the balloon/toast.</param>
        /// <param name="icon">Icon accompanying the popup balloon/toast.</param>
        /// <param name="clickEvent">Click event handler for the popup balloon/toast.</param>
        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToastIconType icon, ToastClickEvent clickEvent)
        {
            var nIconData = new Windows.Shell32Structures.NotifyIconData
            {
                CallbackMessage = (int)clickEvent,
                Handle = hWnd,
                UId = uId,
                Flags = (int)Shell32.NotifyIconFlags.NifMessage | (int)Shell32.NotifyIconFlags.NifInfo,
                Timeout = timeout,
                InfoTitle = tipTitle,
                Info = tipText,
                BalloonIconHandle = toastIconHandles[icon],
                InfoFlags = (int)Shell32.NotifyIconInfoFlags.NiifLargeIcon | (int)Shell32.NotifyIconInfoFlags.NiifUser | (int)Shell32.NotifyIconInfoFlags.NiifNoSound,
            };

            Shell32.ShellNotifyIcon(Shell32.NotifyIconMessage.NimModify, nIconData);
        }

        /// <summary>
        /// Remove the icon from the system tray.
        /// </summary>
        public void RemoveFromTray()
        {
            UpdateIcon(IconType.Disconnected, ProductConstants.DefaultSystemTrayTitle, false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose icon elements and remove from system tray.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (true) or from a finalizer (false).</param>
        public void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                iconHandles.Clear();
                icons.Clear();
            }

            RemoveFromTray();

            disposed = true;
        }
    }
}