// <copyright file="SecurityAttributes.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.Windows
{
    /// <summary>
    /// Contains the security descriptor for an object and specifies whether the handle retrieved by specifying this structure is inheritable.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class SecurityAttributes
    {
        /// <summary>
        /// Gets or sets the size, in bytes, of this structure.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the pointer to a SecurityDescriptor structure that controls access to the object. If the value of this member is NULL, the object is assigned the default security descriptor associated with the access token of the calling process.
        /// </summary>
        public IntPtr SecurityDescriptor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the returned handle is inherited when a new process is created. If this member is TRUE, the new process inherits the handle.
        /// </summary>
        public bool InheritHandle { get; set; }
    }
}
