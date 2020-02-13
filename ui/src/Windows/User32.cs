// <copyright file="User32.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// User32.dll pinvoke library.
    /// </summary>
    public class User32
    {
        /// <summary>
        /// WM_SHOW - shows the Window.
        /// </summary>
        public const int WmShow = 0x0018;

        /// <summary>
        /// WM_DESTROY - A message to destroy a window.
        /// </summary>
        public const int WmDestroy = 0x0002;

        /// <summary>
        /// WM_USER - Used to define private messages for use by private window classes.
        /// </summary>
        public const int WmUser = 0x0400;

        /// <summary>
        /// WM_APP - Used to define private message for the application.
        /// </summary>
        public const int WmApp = 0x8000;

        /// <summary>
        /// Message sent when the tray is clicked.
        /// </summary>
        public const int WmTrayMouseMessage = 0x800;

        /// <summary>
        /// NIN_BALLOONUSERCLICK - Sent when a user clicks the balloon/toast.
        /// </summary>
        public const int NinBalloonUserClick = WmUser + 5;

        /// <summary>
        /// Left click mouse hook value.
        /// </summary>
        public const int WhMouseLL = 14;

        /// <summary>
        /// Class already exists Windows Error ID.
        /// </summary>
        public const int ErrorClassAlreadyExists = 0x582;

        /// <summary>
        /// Mouse messages sent by mouse click/scroll events.
        /// </summary>
        public enum MouseMessages
        {
            /// <summary>
            /// Posted when the user presses the left mouse button while the cursor is in the client area of a window.
            /// </summary>
            WM_LBUTTONDOWN = 0x0201,

            /// <summary>
            /// Posted when the user releases the left mouse button while the cursor is in the client area of a window.
            /// </summary>
            WM_LBUTTONUP = 0x0202,

            /// <summary>
            /// Posted to a window when the cursor moves.
            /// </summary>
            WM_MOUSEMOVE = 0x0200,

            /// <summary>
            /// Sent to the focus window when the mouse wheel is rotated.
            /// </summary>
            WM_MOUSEWHEEL = 0x020A,

            /// <summary>
            /// Posted when the user presses the right mouse button while the cursor is in the client area of a window.
            /// </summary>
            WM_RBUTTONDOWN = 0x0204,

            /// <summary>
            /// Posted when the user releases the right mouse button while the cursor is in the client area of a window.
            /// </summary>
            WM_RBUTTONUP = 0x0205,

            /// <summary>
            /// Posted when the user double-clicks the left mouse button while the cursor is in the client area of a window.
            /// </summary>
            WM_LBUTTONDBLCLK = 0x0203,
        }

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure.</param>
        /// <param name="hInstance">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, NotificationArea.Tray.MouseHookDelegate lpfn, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain.
        /// </summary>
        /// <param name="idHook">This parameter is ignored.</param>
        /// <param name="nCode">The hook code passed to the current hook procedure.</param>
        /// <param name="wParam">The wParam value passed to the current hook procedure.</param>
        /// <param name="lParam">The lParam value passed to the current hook procedure.</param>
        /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="idHook">A handle to the hook to be removed.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings.
        /// </summary>
        /// <param name="lpClassName">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function.</param>
        /// <param name="lpWindowName">The window name (the window's title). If this parameter is NULL, all window names match.</param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class name and window name. If the function fails, the return value is NULL.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Sends the specified message to a window or windows.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose window procedure will receive the message.</param>
        /// <param name="msg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information for wParam.</param>
        /// <param name="lParam">Additional message-specific information for lParam.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Registers a window class for subsequent use in calls to the CreateWindow or CreateWindowEx function.
        /// </summary>
        /// <param name="lpWndClass">A pointer to a WNDCLASS structure.</param>
        /// <returns>If the function succeeds, the return value is a class atom that uniquely identifies the class being registered.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern ushort RegisterClassW([In] ref WndClass lpWndClass);

        /// <summary>
        /// Creates an overlapped, pop-up, or child window with an extended window style.
        /// </summary>
        /// <param name="dwExStyle">The extended window style of the window being created.</param>
        /// <param name="lpClassName">A null-terminated string or a class atom created by a previous call to the RegisterClass or RegisterClassEx function.</param>
        /// <param name="lpWindowName">The window name. If the window style specifies a title bar, the window title pointed to by lpWindowName is displayed in the title bar.</param>
        /// <param name="dwStyle">The style of the window being created.</param>
        /// <param name="x">The initial horizontal position of the window.</param>
        /// <param name="y">The initial vertical position of the window.</param>
        /// <param name="nWidth">The width, in device units, of the window.</param>
        /// <param name="nHeight">The height, in device units, of the window.</param>
        /// <param name="hWndParent">A handle to the parent or owner window of the window being created.</param>
        /// <param name="hMenu">A handle to a menu, or specifies a child-window identifier, depending on the window style.</param>
        /// <param name="hInstance">A handle to the instance of the module to be associated with the window.</param>
        /// <param name="lpParam">Pointer to a value to be passed to the window through the CREATESTRUCT structure (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message.</param>
        /// <returns>If the function succeeds, the return value is a handle to the new window. If the function fails, the return value is NULL.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowExW(
           uint dwExStyle,
           [MarshalAs(UnmanagedType.LPWStr)]
           string lpClassName,
           [MarshalAs(UnmanagedType.LPWStr)]
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam
        );

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown.</param>
        /// <returns>If the window was previously visible, the return value is nonzero. If the window was previously hidden, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Calls the default window procedure to provide default processing for any window messages that an application does not process. This function ensures that every message is processed.
        /// </summary>
        /// <param name="hWnd">A handle to the window procedure that received the message.</param>
        /// <param name="msg">The message.</param>
        /// <param name="wParam">Additional message information for wParam.</param>
        /// <param name="lParam">Additional message information for lParam.</param>
        /// <returns>The return value is the result of the message processing and depends on the message.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern System.IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Destroys the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be destroyed.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        /// <summary>
        /// Indicates to the system that a thread has made a request to terminate (quit).
        /// </summary>
        /// <param name="nExitCode">The application exit code. This value is used as the wParam parameter of the WM_QUIT message.</param>
        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        /// <summary>
        /// Contains the window class attributes that are registered by the RegisterClass function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WndClass
        {
            /// <summary>
            /// The class style(s).
            /// </summary>
            public uint Style;

            /// <summary>
            /// A pointer to the window procedure.
            /// </summary>
            public IntPtr WindowProcedure;

            /// <summary>
            /// The number of extra bytes to allocate following the window-class structure.
            /// </summary>
            public int CbClsExtra;

            /// <summary>
            /// The number of extra bytes to allocate following the window instance.
            /// </summary>
            public int CbWndExtra;

            /// <summary>
            /// A handle to the instance that contains the window procedure for the class.
            /// </summary>
            public IntPtr InstanceHandle;

            /// <summary>
            /// A handle to the class icon. This member must be a handle to an icon resource.
            /// </summary>
            public IntPtr IconHandle;

            /// <summary>
            /// A handle to the class cursor. This member must be a handle to a cursor resource.
            /// </summary>
            public IntPtr CursorHandle;

            /// <summary>
            /// A handle to the class background brush.
            /// </summary>
            public IntPtr BackgroundBrushHandle;

            /// <summary>
            /// The resource name of the class menu, as the name appears in the resource file.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string MenuName;

            /// <summary>
            /// A pointer to a null-terminated string or is an atom.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClassName;
        }
    }
}
