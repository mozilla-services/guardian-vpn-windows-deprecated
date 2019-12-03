// <copyright file="IPC.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FirefoxPrivateNetwork.Windows;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// IPC class, used for handling communication within pipes between the main app and the broker process.
    /// </summary>
    public class IPC
    {
        private const uint BufferSize = 512;

        private const uint ErrorBrokenPipe = 0x6D;
        private const uint ErrorMoreData = 0xEA;

        private readonly IntPtr readPipe;
        private readonly IntPtr writePipe;

        private Thread listener = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPC"/> class.
        /// </summary>
        /// <param name="readPipe">Read pipe handle.</param>
        /// <param name="writePipe">Write pipe handle.</param>
        public IPC(IntPtr readPipe, IntPtr writePipe)
        {
            this.IsActive = true;
            this.readPipe = readPipe;
            this.writePipe = writePipe;
        }

        /// <summary>
        /// Gets a value indicating whether the IPC pipe is active or not.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Writes a message to a named pipe.
        /// </summary>
        /// <param name="pipe">Named pipe instance to write to.</param>
        /// <param name="message">IPCMessage to send.</param>
        public static void WriteToPipe(NamedPipeClientStream pipe, IPCMessage message)
        {
            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            pipe.Write(bytes, 0, bytes.Length);
            pipe.Flush();
        }

        /// <summary>
        /// Reads a message from a named Windows pipe.
        /// </summary>
        /// <param name="pipe">Instance of a named pipe client stream.</param>
        /// <returns>Retrieved message from the pipe.</returns>
        public static IPCMessage ReadFromPipe(NamedPipeClientStream pipe)
        {
            var readBytes = new List<byte>();
            byte[] buffer = new byte[BufferSize];

            while (true)
            {
                int numBytesRead = pipe.Read(buffer, 0, buffer.Length);
                if (numBytesRead == 0)
                {
                    break;
                }

                var foundFirst = false;
                if (readBytes.Count > 0)
                {
                    foundFirst = readBytes.Last() == '\n';
                }

                var done = false;
                for (var i = 0; i < numBytesRead; ++i)
                {
                    readBytes.Add(buffer[i]);
                    if (buffer[i] == '\n')
                    {
                        if (foundFirst)
                        {
                            done = true;
                            break;
                        }

                        foundFirst = true;
                    }
                    else
                    {
                        foundFirst = false;
                    }
                }

                if (done)
                {
                    break;
                }
            }

            return new IPCMessage(Encoding.UTF8.GetString(readBytes.ToArray()));
        }

        /// <summary>
        /// Writes a message to the instantiated Windows pipe.
        /// </summary>
        /// <param name="message">IPC message to write.</param>
        /// <returns>True on success.</returns>
        public bool WriteToPipe(IPCMessage message)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message.ToString());
                var writeResult = Kernel32.WriteFile(this.writePipe, buffer, (uint)buffer.Length, out uint bytesWritten, IntPtr.Zero);
                if (!writeResult)
                {
                    var lastWindowsError = Marshal.GetLastWin32Error();
                    if (lastWindowsError == ErrorBrokenPipe)
                    {
                        ErrorHandling.ErrorHandler.Handle("Could not write to pipe, got ERROR_BROKEN_PIPE from Windows", ErrorHandling.LogLevel.Debug);
                        this.StopListenerThread();
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads a message from an instantiated Windows pipe.
        /// </summary>
        /// <returns>Retrieved IPC message from the pipe.</returns>
        public IPCMessage ReadFromPipe()
        {
            var readBytes = new List<byte>();
            byte[] buffer = new byte[BufferSize];
            int lastWindowsError;

            while (true)
            {
                bool readSuccess = Kernel32.ReadFile(this.readPipe, buffer, BufferSize, out uint numBytesRead, IntPtr.Zero);
                lastWindowsError = Marshal.GetLastWin32Error();
                if (!readSuccess && lastWindowsError != ErrorMoreData)
                {
                    break;
                }

                var foundFirst = false;
                if (readBytes.Count > 0)
                {
                    foundFirst = readBytes.Last() == '\n';
                }

                var done = false;
                for (var i = 0; i < numBytesRead; ++i)
                {
                    readBytes.Add(buffer[i]);
                    if (buffer[i] == '\n')
                    {
                        if (foundFirst)
                        {
                            done = true;
                            break;
                        }

                        foundFirst = true;
                    }
                    else
                    {
                        foundFirst = false;
                    }
                }

                if (done)
                {
                    break;
                }
            }

            if (lastWindowsError == ErrorBrokenPipe)
            {
                ErrorHandling.ErrorHandler.Handle("Could not read from pipe, got ERROR_BROKEN_PIPE from Windows", ErrorHandling.LogLevel.Error);
                this.StopListenerThread();
                return new IPCMessage();
            }

            return new IPCMessage(Encoding.UTF8.GetString(readBytes.ToArray()));
        }

        /// <summary>
        /// Initiates an IPC listener thread with this instance of IPC.
        /// </summary>
        public void StartListenerThread()
        {
            IsActive = true;
            listener = new Thread(new ThreadStart(ListenerThread))
            {
                IsBackground = true,
            };
            listener.Start();
        }

        /// <summary>
        /// Checks if the listener thread is active.
        /// </summary>
        /// <returns>Returns true or false, based on whether the listener is active.</returns>
        public bool IsListenerActive()
        {
            return listener.IsAlive;
        }

        /// <summary>
        /// Stops the listener thread, if active.
        /// </summary>
        public void StopListenerThread()
        {
            IsActive = false;
        }

        /// <summary>
        /// Listener thread, in which we continue looping until the app terminates or IsActive is false.
        /// </summary>
        private void ListenerThread()
        {
            while (IsActive)
            {
                IPCHandlers.HandleIncomingMessage(ReadFromPipe(), this);
            }
        }
    }
}
