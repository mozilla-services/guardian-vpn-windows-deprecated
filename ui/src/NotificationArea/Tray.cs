// <copyright file="Tray.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FirefoxPrivateNetwork.NotificationArea
{
    /// <summary>
    /// Handles the tray icon in the notification area.
    /// </summary>
    public class Tray
    {
        private const int DefaultBalloonTipTimeout = 4000;

        private readonly TrayMessageWindow messageWindow;
        private readonly NotifyIconCustom notifyIcon;
        private readonly MouseHookDelegate mouseHookDelegate;
        private ContextMenu contextMenu;
        private MenuItem menuConnectionStatus;

        private int outsideClickHook = 0;
        private string connectionStatusText;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tray"/> class.
        /// </summary>
        /// <param name="window">TrayMessageWindow to use as a message handler.</param>
        public Tray(TrayMessageWindow window)
        {
            SetupMenu(false);
            messageWindow = window;
            notifyIcon = new NotifyIconCustom(messageWindow.GetHandle(), ProductConstants.DefaultSystemTrayTitle);

            // Setup hook delegate for outside clicks
            mouseHookDelegate = new MouseHookDelegate(MouseHookHandler);
        }

        /// <summary>
        /// Delegate to handle mouse hook messages (win32 API).
        /// </summary>
        /// <param name="nCode">A code the hook procedure uses to determine how to process the message.</param>
        /// <param name="wParam">The identifier of the mouse message.</param>
        /// <param name="lParam">A pointer to an MSLLHOOKSTRUCT structure.</param>
        /// <returns>If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        /// If nCode is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns;
        /// otherwise, other applications that have installed WH_MOUSE_LL hooks will not receive hook notifications and may behave incorrectly as a result.If the hook procedure processed the message,
        /// it may return a nonzero value to prevent the system from passing the message to the rest of the hook chain or the target window procedure.
        /// </returns>
        public delegate int MouseHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Show the MainWindow UI.
        /// </summary>
        public static void ShowMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
            {
                mainWindow = new UI.MainWindow();
            }
            else if (mainWindow.GetType() != typeof(UI.MainWindow))
            {
                Application.Current.MainWindow.Close();
                mainWindow = new UI.MainWindow();
            }

            mainWindow.Show();
            mainWindow.Activate();
            mainWindow.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Hides the MainWindow UI.
        /// </summary>
        public static void HideMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Close();
            }
        }

        /// <summary>
        /// Handle mouse hook messages (win32 API).
        /// </summary>
        /// <param name="nCode">A code the hook procedure uses to determine how to process the message.</param>
        /// <param name="wParam">The identifier of the mouse message.</param>
        /// <param name="lParam">A pointer to an MSLLHOOKSTRUCT structure.</param>
        /// <returns>If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        /// If nCode is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns;
        /// otherwise, other applications that have installed WH_MOUSE_LL hooks will not receive hook notifications and may behave incorrectly as a result.If the hook procedure processed the message,
        /// it may return a nonzero value to prevent the system from passing the message to the rest of the hook chain or the target window procedure.
        /// </returns>
        public int MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                switch ((int)wParam)
                {
                    case (int)Windows.User32.MouseMessages.WM_LBUTTONDOWN:
                    case (int)Windows.User32.MouseMessages.WM_LBUTTONUP:
                    case (int)Windows.User32.MouseMessages.WM_RBUTTONDOWN:
                    case (int)Windows.User32.MouseMessages.WM_RBUTTONUP:
                        if (contextMenu.IsMouseDirectlyOver)
                        {
                            TearDownGlobalMouseHook();
                            contextMenu.IsOpen = false;
                        }

                        break;
                    default:

                        break;
                }
            }

            return Windows.User32.CallNextHookEx(outsideClickHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// Handle mouse hook message received.
        /// </summary>
        /// <param name="msg">Message ID received.</param>
        public void HandleMessage(long msg)
        {
            switch (msg)
            {
                case (long)Windows.User32.MouseMessages.WM_LBUTTONUP:
                    ShowMainWindow();
                    break;
                case (long)Windows.User32.MouseMessages.WM_LBUTTONDBLCLK:
                    ShowMainWindow();
                    break;
                case (long)Windows.User32.MouseMessages.WM_RBUTTONUP:
                    menuConnectionStatus.Header = connectionStatusText;
                    TearDownGlobalMouseHook();
                    contextMenu.IsOpen = true;
                    SetupGlobalMouseHook();
                    break;
                case Windows.User32.NinBalloonUserClick:
                    ShowMainWindow();
                    break;
            }
        }

        /// <summary>
        /// Sets the TrayIcon to have a "disconnected" image as its icon.
        /// </summary>
        public void SetDisconnected()
        {
            var newText = string.Concat(ProductConstants.DefaultSystemTrayTitle, " - ", "Disconnected");
            SetContextMenuStatus(newText);
            notifyIcon.UpdateIcon(IconType.Disconnected, newText);
        }

        /// <summary>
        /// Sets the TrayIcon to have a "connected" image as its icon.
        /// </summary>
        public void SetConnected()
        {
            var newText = string.Concat(ProductConstants.DefaultSystemTrayTitle, " - ", "Connected");
            SetContextMenuStatus(newText);
            notifyIcon.UpdateIcon(IconType.Connected, newText);
        }

        /// <summary>
        /// Sets the TrayIcon to have an "idle" image as its icon.
        /// </summary>
        public void SetIdle()
        {
            var newText = string.Concat(ProductConstants.DefaultSystemTrayTitle, " - ", "Idle");
            SetContextMenuStatus(newText);
            notifyIcon.UpdateIcon(IconType.Unstable, newText);
        }

        /// <summary>
        /// Sets the TrayIcon to have an "error" image as its icon.
        /// </summary>
        public void SetUnstable()
        {
            var newText = string.Concat(ProductConstants.DefaultSystemTrayTitle, " - ", "No Signal");
            SetContextMenuStatus(newText);
            notifyIcon.UpdateIcon(IconType.NoSignal, newText);
        }

        /// <summary>
        /// Show a balloon notification popup.
        /// </summary>
        /// <param name="notificationTitle">Balloon title.</param>
        /// <param name="notificationText">Text to show inside the balloon.</param>
        /// <param name="icon">Type of the icon to show.</param>
        /// <param name="timeoutMiliseconds">Timeout in miliseconds after which the balloon will be hidden.</param>
        /// <param name="clickEvent">Click event handler type when the balloon is clicked.</param>
        public void ShowNotification(string notificationTitle, string notificationText, ToastIconType icon, int timeoutMiliseconds = DefaultBalloonTipTimeout, ToastClickEvent clickEvent = ToastClickEvent.None)
        {
            notifyIcon.ShowBalloonTip(timeoutMiliseconds, notificationTitle, notificationText, icon, clickEvent);
        }

        /// <summary>
        /// Handle a balloon notification popup click.
        /// </summary>
        /// <param name="msg">Message ID received.</param>
        public void HandleNotificationClick(uint msg)
        {
            ToastClickEvent clickEvent;

            try
            {
                clickEvent = (ToastClickEvent)msg;
            }
            catch (Exception)
            {
                return;
            }

            switch (clickEvent)
            {
                case ToastClickEvent.Connect:
                    ShowMainWindow();
                    WireGuard.Connector.Connect();
                    break;
                case ToastClickEvent.Disconnect:
                    WireGuard.Connector.Disconnect();
                    break;
                default:
                    ShowMainWindow();
                    break;
            }
        }

        /// <summary>
        /// Remove and dispose of the tray icon.
        /// </summary>
        public void Remove()
        {
            notifyIcon.RemoveFromTray();
            messageWindow.Dispose();
            TearDownGlobalMouseHook();
        }

        /// <summary>
        /// Sets the context menu connection status text.
        /// </summary>
        /// <param name="text">Text to show as the connection status in the context menu.</param>
        private void SetContextMenuStatus(string text)
        {
            connectionStatusText = text;
        }

        /// <summary>
        /// Create a new hook which listens to mouse events.
        /// </summary>
        private void SetupGlobalMouseHook()
        {
            Process curProcess = Process.GetCurrentProcess();
            ProcessModule curModule = curProcess.MainModule;
            outsideClickHook = Windows.User32.SetWindowsHookEx(Windows.User32.WhMouseLL, mouseHookDelegate, Windows.Kernel32.GetModuleHandle(curModule.ModuleName), 0);
        }

        /// <summary>
        /// Remove mouse hook, if not already removed.
        /// </summary>
        private void TearDownGlobalMouseHook()
        {
            if (outsideClickHook != 0)
            {
                Windows.User32.UnhookWindowsHookEx(outsideClickHook);
                outsideClickHook = 0;
            }
        }

        /// <summary>
        /// Setup the right click menu for the Tray.
        /// </summary>
        private void SetupMenu(bool connectionActive)
        {
            // "Exit" menu item
            var menuExit = new System.Windows.Controls.MenuItem
            {
                Header = "_Exit",
            };
            menuExit.Click += (sender, e) =>
            {
                HideMainWindow();
                Application.Current.Shutdown();
            };

            // "Show" menu item
            var menuShow = new MenuItem
            {
                Header = "_Show",
            };
            menuShow.Click += (sender, e) =>
            {
                ShowMainWindow();
            };

            // "Hide" menu item
            var menuHide = new MenuItem
            {
                Header = "_Hide",
            };
            menuHide.Click += (sender, e) =>
            {
                HideMainWindow();
            };

            // App title and connection status
            connectionStatusText = string.Concat(ProductConstants.DefaultSystemTrayTitle, " - ", connectionActive ? "Connected" : "Disconnected");
            menuConnectionStatus = new MenuItem()
            {
                Header = connectionStatusText,
                IsEnabled = false,
            };

            contextMenu = new ContextMenu();

            contextMenu.Items.Add(menuConnectionStatus);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuShow);
            contextMenu.Items.Add(menuHide);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(menuExit);
        }
    }
}
