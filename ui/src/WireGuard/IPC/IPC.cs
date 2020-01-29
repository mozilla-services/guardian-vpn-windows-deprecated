// <copyright file="IPC.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// IPC class, used for handling communication within pipes between the main app and the broker process.
    /// </summary>
    public class IPC
    {
        private const uint BufferSize = 512;
        private readonly PipeStream pipe;
        private Thread listener = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPC"/> class.
        /// </summary>
        /// <param name="pipe">Pipe handle.</param>
        public IPC(NamedPipeServerStream pipe)
        {
            this.pipe = pipe;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPC"/> class.
        /// </summary>
        /// <param name="pipe">Pipe handle.</param>
        public IPC(NamedPipeClientStream pipe)
        {
            this.pipe = pipe;
        }

        /// <summary>
        /// Writes a message to a named pipe.
        /// </summary>
        /// <param name="pipe">Named pipe instance to write to.</param>
        /// <param name="message">IPCMessage to send.</param>
        public static void WriteToPipe(NamedPipeClientStream pipe, IPCMessage message)
        {
            if (!pipe.IsConnected)
            {
                pipe.Connect();
            }

            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            pipe.Write(bytes, 0, bytes.Length);
            pipe.Flush();
        }

        /// <summary>
        /// Reads a message from a named Windows pipe.
        /// </summary>
        /// <param name="pipe">Instance of a named pipe stream.</param>
        /// <param name="readAsync">Flag that indicates asynchronous/synchronous reading from the pipe.</param>
        /// <returns>Retrieved message from the pipe.</returns>
        public static IPCMessage ReadFromPipe(PipeStream pipe, bool readAsync = false)
        {
            var readBytes = new List<byte>();
            byte[] buffer = new byte[BufferSize];

            while (true)
            {
                int numBytesRead;
                if (!readAsync)
                {
                    numBytesRead = pipe.Read(buffer, 0, buffer.Length);
                }
                else
                {
                    numBytesRead = PipeReadAsyncWrapper(pipe, buffer).Result;
                }

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
        /// <param name="promptRestartBrokerServiceOnFail">Prompt the user to restart a stopped broker service.</param>
        /// <returns>True on success.</returns>
        public bool WriteToPipe(IPCMessage message, bool promptRestartBrokerServiceOnFail = true)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(message.ToString());
                pipe.Write(buffer, 0, buffer.Length);
                pipe.Flush();
            }
            catch (Exception e)
            {
                if (pipe is NamedPipeClientStream && (e is InvalidOperationException || e is IOException))
                {
                    if (promptRestartBrokerServiceOnFail)
                    {
                        Broker.PromptRestartBrokerService();
                    }
                }

                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initiates an IPC listener thread with this instance of IPC.
        /// </summary>
        public void StartClientListenerThread()
        {
            listener = new Thread(() =>
            {
                ClientListenerThread();
                ClientListenerThreadTerminated();
            })
            {
                IsBackground = true,
            };
            listener.Start();
        }

        /// <summary>
        /// Broker listener thread, in which we continue listening for messages from the client until the app terminates or a cancellation has been requested.
        /// </summary>
        public void BrokerListenerThread()
        {
            // Wait for a client to connect to the broker pipe
            try
            {
                if (WaitForConnectionAsyncWrapper().Result)
                {
                    Broker.StartChildProcess();
                }
            }
            catch (Exception e)
            {
                ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
                return;
            }

            // Handle incoming messages from the pipe while it is connected
            while (!BrokerService.BrokerServiceTokenSource.IsCancellationRequested && pipe.IsConnected)
            {
                try
                {
                    IPCHandlers.HandleIncomingMessage(ReadFromPipe(pipe, readAsync: true), this);
                }
                catch (Exception e)
                {
                    ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Debug);
                    break;
                }
            }
        }

        private static async Task<int> PipeReadAsyncWrapper(PipeStream pipe, byte[] buffer)
        {
            // Initialize a task completion source for the pipe read task
            var completionSource = new TaskCompletionSource<object>();

            // Register a method to cancel the task completion source with a cancellation is requested through the broker service's cancellation token source
            var tokenRegistration = BrokerService.BrokerServiceTokenSource.Token.Register(() => completionSource.TrySetCanceled());

            // Start the pipe read task
            var task = Task<int>.Factory.StartNew(() =>
            {
                return pipe.Read(buffer, 0, buffer.Length);
            }, BrokerService.BrokerServiceTokenSource.Token);

            // Wait for either the pipe read task to complete, or the task completion source has been canceled
            var completedTask = await Task.WhenAny(task, completionSource.Task);

            // Dispose the registered method in the broker service's cancellation token source if the pipe read task completed successfully
            if (completedTask == task)
            {
                tokenRegistration.Dispose();
                return task.Result;
            }

            return 0;
        }

        /// <summary>
        /// Client listener thread, in which we continue listening for message from the broker until the app terminates or a cancellation has been requested.
        /// </summary>
        private void ClientListenerThread()
        {
            while (true)
            {
                if (!pipe.IsConnected)
                {
                    try
                    {
                        ((NamedPipeClientStream)pipe).Connect();
                    }
                    catch (Exception e)
                    {
                        ErrorHandling.ErrorHandler.Handle(e, ErrorHandling.LogLevel.Error);
                        continue;
                    }
                }

                IPCHandlers.HandleIncomingMessage(ReadFromPipe(pipe), this);
            }
        }

        private void ClientListenerThreadTerminated()
        {
            ((NamedPipeClientStream)pipe).Close();
            ((NamedPipeClientStream)pipe).Dispose();
        }

        private async Task<bool> WaitForConnectionAsyncWrapper()
        {
            await ((NamedPipeServerStream)pipe).WaitForConnectionAsync(BrokerService.BrokerServiceTokenSource.Token);
            return pipe.IsConnected;
        }
    }
}
