// <copyright file="TrayMessageWindow.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.NotificationArea
{
    /// <summary>
    /// Tray message handler window. Used for receiving messages sent to the application's tray icon.
    /// </summary>
    public class TrayMessageWindow : IDisposable
    {
        private readonly WndProc windowProcDelegate;
        private bool isDisposed;
        private IntPtr windowHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayMessageWindow"/> class.
        /// </summary>
        /// <param name="className">Name of the window class to be used with this window.</param>
        public TrayMessageWindow(string className)
        {
            if (className == null || className == string.Empty)
            {
                throw new System.Exception("Provided class name cannot be empty");
            }

            windowProcDelegate = TrayWndProc;
            var windowClass = new User32.WndClass
            {
                ClassName = className,
                WindowProcedure = Marshal.GetFunctionPointerForDelegate(windowProcDelegate),
            };

            // Register the class name
            ushort classId = User32.RegisterClassW(ref windowClass);
            if (classId == 0 && Marshal.GetLastWin32Error() != User32.ErrorClassAlreadyExists)
            {
                ErrorHandling.ErrorHandler.Handle("Cannot register Tray Window class", ErrorHandling.LogLevel.Error);
                Environment.Exit(1);
            }

            // Finally, create the window
            windowHandle = User32.CreateWindowExW(
                0,
                className,
                string.Empty,
                0,
                0,
                0,
                0,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieves the current hWnd of the instantiated window.
        /// </summary>
        /// <returns>hWnd pointer to the current instantiated window.</returns>
        public IntPtr GetHandle()
        {
            return windowHandle;
        }

        /// <summary>
        /// Window process which handles messages sent to the system tray.
        /// </summary>
        /// <param name="hWnd">Window handle to use.</param>
        /// <param name="msg">Type of message received.</param>
        /// <param name="wParam">Win32 wParam object. Contents depend on msg.</param>
        /// <param name="lParam">Win32 lParam object. Contents depend on msg.</param>
        /// <returns>Null pointer or the result of the default window procedure which ensures every message is processed.</returns>
        private static IntPtr TrayWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (lParam == new IntPtr(Windows.User32.NinBalloonUserClick))
            {
                Manager.TrayIcon.HandleNotificationClick(msg);
            }

            switch (msg)
            {
                case Windows.User32.WmDestroy:
                    User32.PostQuitMessage(0);
                    return IntPtr.Zero;

                case Windows.User32.WmShow:
                    Tray.ShowMainWindow();
                    return IntPtr.Zero;

                case Windows.User32.WmTrayMouseMessage:
                    if (Manager.TrayIcon != null)
                    {
                        Manager.TrayIcon.HandleMessage(lParam.ToInt64());
                    }

                    return IntPtr.Zero;
                default:
                    return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
            }
        }

        /// <summary>
        /// Destroys the window operating as the message handler.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (true) or from a finalizer (false).</param>
        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed resources
            }

            // Dispose unmanaged resources
            if (windowHandle != IntPtr.Zero)
            {
                User32.DestroyWindow(windowHandle);
                windowHandle = IntPtr.Zero;
            }

            isDisposed = true;
        }
    }
}
