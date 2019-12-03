// <copyright file="Balrog.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright(C) 2019 Edge Security LLC. All Rights Reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.Update
{
    /// <summary>
    /// Balrog interfacing functionality.
    /// </summary>
    internal class Balrog
    {
        /// <summary>
        /// The root fingerprint for the Balrog Moz root cert.
        /// </summary>
        public
#if DEBUG_QA
        static
#else
        const
#endif
        string UpdateRootFingerprint = "97:E8:BA:9C:F1:2F:B3:DE:53:CC:42:A4:E6:57:7E:D6:4D:F4:93:C2:47:B4:14:FE:A0:36:81:8D:38:23:56:0E";

        private const string BalrogNT32 = "WINNT_x86_32";
        private const string BalrogNT64 = "WINNT_x86_64";
        private const string X509Url = "x5u";
        private const int MaxJsonDownloadSize = 1024 * 1024; // 1MiB
        private const int MaxHttpRetries = 2;

        // Cert subject for Balrog.
        private const string UpdateCertSubject = "CN=aus.content-signature.mozilla.org";

        // OID for Balrog certificates.
        private const string UpdateCodeSigningOid = "1.3.6.1.5.5.7.3.3";

        private static readonly Dictionary<string, HashAlgorithmName> SignatureHashAlgorithms = new Dictionary<string, HashAlgorithmName>()
        {
            { "p256ecdsa", HashAlgorithmName.SHA256 },
            { "p384ecdsa", HashAlgorithmName.SHA384 },
        };

        // Client for sending HTTP requests.
        private static readonly UpdateHttpClient UpdateHttpClient = new UpdateHttpClient(MaxJsonDownloadSize);

        /// <summary>
        /// Queries for available updates from Balrog.
        /// </summary>
        /// <param name="currentVersion">current version.</param>
        /// <returns>Balrog response structure.</returns>
        public static async Task<JSONStructures.BalrogResponse> QueryUpdate(string currentVersion)
        {
            var updateUrl = GetUpdateUrl(currentVersion);
            string contentSignature;
            byte[] jsonContentsBlob;

            try
            {
                using (var response = await UpdateHttpClient.QueryWithRetryAsync(updateUrl, MaxHttpRetries))
                {
                    // Check if a non-success status code is returned from the remote server
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    // Getting the content signature from the response headers
                    contentSignature = response.Headers.TryGetValues("Content-Signature", out var signatures) ? signatures.FirstOrDefault() : null;
                    jsonContentsBlob = await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (HttpRequestException e)
            {
                ErrorHandling.ErrorHandler.Handle(string.Concat("Query update error: ", e.Message), ErrorHandling.LogLevel.Error);
                return null;
            }

            try
            {
                ErrorHandling.ErrorHandler.Handle("Verifying the content signature", ErrorHandling.LogLevel.Info);
                var isSignatureValid = await CheckSignature(contentSignature, jsonContentsBlob);
                if (!isSignatureValid)
                {
                    ErrorHandling.ErrorHandler.Handle("Content signature was not valid.", ErrorHandling.LogLevel.Error);
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                ErrorHandling.ErrorHandler.Handle(string.Concat("Content signature verification error: ", e.Message), ErrorHandling.LogLevel.Error);
                return null;
            }

            // Signature is valid, parse JSON
            JSONStructures.BalrogResponse balrogResponse;
            try
            {
                balrogResponse = JsonConvert.DeserializeObject<JSONStructures.BalrogResponse>(Encoding.UTF8.GetString(jsonContentsBlob));
            }
            catch (Exception)
            {
                ErrorHandling.ErrorHandler.Handle("Unable to parse balrog response from JSON.", ErrorHandling.LogLevel.Error);
                return null;
            }

            // Downgrade check
            if (new Version(balrogResponse.LatestVersion).CompareTo(new Version(currentVersion)) <= 0)
            {
                ErrorHandling.ErrorHandler.Handle("Update version was not greater than the current version. Stopping.", ErrorHandling.LogLevel.Error);
                return null;
            }

            // Update is available, show notification
            return balrogResponse;
        }

        private static string GetUpdateUrl(string currentVersion)
        {
            return string.Format(ProductConstants.UpdateTemplateUrl, currentVersion, GetBalrogUserAgent());
        }

        private static string GetBalrogUserAgent()
        {
            return Environment.Is64BitProcess ? BalrogNT64 : BalrogNT32;
        }

        private static async Task<bool> CheckSignature(string contentSignature, byte[] jsonContentsBlob)
        {
            if (string.IsNullOrWhiteSpace(contentSignature) || jsonContentsBlob.Length == 0)
            {
                return false;
            }

            var signature = default(Signature);
            try
            {
                signature = ParseContentSignatureElements(contentSignature);
            }
            catch (ArgumentException)
            {
                return false;
            }

            // Download chain file
            string chainContents;
            using (var chainResponse = await UpdateHttpClient.QueryWithRetryAsync(signature.X509Url, MaxHttpRetries))
            {
                if (!chainResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                chainContents = await chainResponse.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(chainContents))
                {
                    return false;
                }
            }

            // Parse chain file
            var x509Chain = new X509Chain();
            try
            {
                x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                x509Chain.ChainPolicy.ApplicationPolicy.Add(new Oid(UpdateCodeSigningOid));

                X509Certificate2 x509Leaf = null;

                var chainCerts = Regex.Split(chainContents, @"(?=-----BEGIN CERTIFICATE-----\n)").Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

                var x509ChainCerts = new List<X509Certificate2>();
                chainCerts.ForEach(chainCert =>
                {
                    x509ChainCerts.Add(new X509Certificate2(Encoding.UTF8.GetBytes(chainCert)));
                });

                // Get the leaf certificate
                x509Leaf = x509ChainCerts.FirstOrDefault();
                if (x509Leaf == null)
                {
                    return false;
                }

                // All other certs
                x509ChainCerts.GetRange(1, x509ChainCerts.Count - 1).ForEach(cert =>
                {
                    if (cert != null)
                    {
                        x509Chain.ChainPolicy.ExtraStore.Add(cert);
                    }
                });

                // Attempt to build chain
                if (!x509Chain.Build(x509Leaf))
                {
                    return false;
                }

                // Check whether all certs within the chain (with the leaf) were built successfully
                if (x509Chain.ChainElements.Count != x509Chain.ChainPolicy.ExtraStore.Count + 1)
                {
                    return false;
                }

                // Check root certificate
                var rootCert = x509Chain.ChainElements[x509Chain.ChainElements.Count - 1];
                using (var sha256Hasher = SHA256.Create())
                {
                    var w = BitConverter.ToString(sha256Hasher.ComputeHash(rootCert.Certificate.RawData)).Replace("-", ":");
                    if (w != UpdateRootFingerprint)
                    {
                        return false;
                    }
                }

                // Validate cert subject
                if (x509Leaf.SubjectName.Decode(X500DistinguishedNameFlags.UseNewLines).Split(new[] { Environment.NewLine }, StringSplitOptions.None).FirstOrDefault((str) => str.TrimStart().StartsWith("CN=")) != UpdateCertSubject)
                {
                    return false;
                }

                // Verify JSON data
                var ecdsaPublicKey = x509Leaf.GetECDsaPublicKey();
                if (ecdsaPublicKey == null)
                {
                    return false;
                }

                byte[] verificationData = Encoding.UTF8.GetBytes("Content-Signature:").Concat(new byte[] { 0 }).Concat(jsonContentsBlob).ToArray();
                if (!ecdsaPublicKey.VerifyData(verificationData, signature.SignatureBlob, signature.HashAlgorithm))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                x509Chain.Dispose();
            }
        }

        private static Signature ParseContentSignatureElements(string contentSignature)
        {
            var signature = default(Signature);
            var signatureElements = contentSignature.Split(';').Select(item => item.Trim()).Select(item => item.Split('=')).ToDictionary(item => item[0], item => item[1]);

            string signatureCurve = null;
            foreach (var curve in SignatureHashAlgorithms)
            {
                signatureElements.TryGetValue(curve.Key, out signatureCurve);
                if (!string.IsNullOrWhiteSpace(signatureCurve))
                {
                    // Set the hash algorithm name based on the found curve
                    signature.HashAlgorithm = curve.Value;
                    break;
                }
            }

            // Have we found a signature curve to use?
            if (string.IsNullOrWhiteSpace(signatureCurve))
            {
                throw new ArgumentException("Empty arguments provided for signature.");
            }

            // Verify received data
            if (!signatureElements.ContainsKey(X509Url) || string.IsNullOrWhiteSpace(signatureElements[X509Url]))
            {
                throw new ArgumentException("Empty arguments provided for x509 URL.");
            }

            // Decode signature
            signature.SignatureBlob = Convert.FromBase64String(Base64SafeUrlDecode(signatureCurve));

            // Set X509 Url
            signature.X509Url = signatureElements[X509Url];

            return signature;
        }

        private static string Base64SafeUrlDecode(string base64)
        {
            return base64.Replace("_", "/").Replace("-", "+") + new string('=', (4 - (base64.Length % 4)) % 4);
        }

        private struct Signature
        {
            public byte[] SignatureBlob;
            public string X509Url;
            public HashAlgorithmName HashAlgorithm;
        }
    }
}
