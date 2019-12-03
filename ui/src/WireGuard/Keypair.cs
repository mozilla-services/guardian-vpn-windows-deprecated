// <copyright file="Keypair.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

/* SPDX-License-Identifier: MIT
 *
 * Copyright (C) 2019 Edge Security LLC. All Rights Reserved.
 */

using System;
using System.Runtime.InteropServices;

namespace FirefoxPrivateNetwork.WireGuard
{
    /// <summary>
    /// Contains DH Curve25519 based public and private keys generated for WireGuard.
    /// </summary>
    public class Keypair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Keypair"/> class.
        /// </summary>
        /// <param name="publicKey">Public key to use.</param>
        /// <param name="privateKey">Private key to use.</param>
        public Keypair(string publicKey, string privateKey)
        {
            Public = publicKey;
            Private = privateKey;
        }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        public string Public { get; set; }

        /// <summary>
        /// Gets or sets the private key.
        /// </summary>
        public string Private { get; set; }

        /// <summary>
        /// Generate a new key pair.
        /// </summary>
        /// <returns>Keypair object containing a public and private key.</returns>
        public static Keypair Generate()
        {
            var publicKey = new byte[32];
            var privateKey = new byte[32];

            WireGuardGenerateKeypair(publicKey, privateKey);
            return new Keypair(Convert.ToBase64String(publicKey), Convert.ToBase64String(privateKey));
        }

        [DllImport("tunnel.dll", EntryPoint = "WireGuardGenerateKeypair", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool WireGuardGenerateKeypair(byte[] publicKey, byte[] privateKey);
    }
}
