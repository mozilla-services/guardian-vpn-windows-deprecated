// <copyright file="IPCMessage.cs" company="Mozilla">
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
    /// IPC Message structure for.
    /// </summary>
    public class IPCMessage : List<(string, string)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPCMessage"/> class.
        /// </summary>
        public IPCMessage() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPCMessage"/> class.
        /// </summary>
        /// <param name="input">Newline character delimited string to use as a parameter list.</param>
        public IPCMessage(string input) : base()
        {
            foreach (var line in input.Split(new char[] { '\n' }, StringSplitOptions.None))
            {
                if (line.Length < 2 || !AddAttribute(line))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Indexer method for IPCMessage.
        /// </summary>
        /// <param name="key">IPC Message type to access.</param>
        /// <returns>List of strings containing the IPC message.</returns>
        public List<string> this[string key]
        {
            get
            {
                /* If profiling actually indicates this is slow, we can cache this in a hashtable. */
                var list = new List<string>();
                foreach (var item in this)
                {
                    if (item.Item1 == key)
                    {
                        list.Add(item.Item2);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Adds a new attribute to the IPC message.
        /// </summary>
        /// <param name="line">Paremeter contents to add.</param>
        /// <example>
        /// <code>
        /// msg.AddAttribute("world=hello");
        /// </code>
        /// </example>
        /// <returns>Returns true on success.</returns>
        public bool AddAttribute(string line)
        {
            var parts = line.Split(new char[] { '=' }, 2);
            if (parts.Length != 2)
            {
                return false;
            }

            Add((parts[0], parts[1]));
            return true;
        }

        /// <summary>
        /// Adds a new attribute to the IPC message.
        /// </summary>
        /// <example>
        /// <code>
        /// msg.AddAttribute("world", "hello");
        /// </code>
        /// </example>
        /// <param name="key">Key value of IPC attribute.</param>
        /// <param name="val">Value of the IPC attribute.</param>
        /// <returns>Returns true on success.</returns>
        public bool AddAttribute(string key, string val)
        {
            Add((key, val));
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var returnString = new StringBuilder();
            foreach (var line in this)
            {
                returnString.AppendFormat("{0}={1}\n", line.Item1, line.Item2);
            }

            returnString.Append("\n");
            return returnString.ToString();
        }
    }
}
