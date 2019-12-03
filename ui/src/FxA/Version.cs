// <copyright file="Version.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Represents the application version, parsed from the executing assembly version.
    /// </summary>
    public class Version : IComparable
    {
        private readonly int major;
        private readonly int minor;
        private readonly int release;

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="majorVersion">16-bit number identifying the major version.</param>
        /// <param name="minorVersion">16-bit number identifying the minor version.</param>
        /// <param name="releaseIdentifier">Release identifier indicating whether this application is a beta (1), alpha (2) or release (0) version.</param>
        public Version(int majorVersion, int minorVersion, int releaseIdentifier)
        {
            major = majorVersion;
            minor = minorVersion;
            release = releaseIdentifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="version">A string indicating the application version (i.e. 0.13b).</param>
        public Version(string version)
        {
            var verSplit = version.Split('.');

            if (verSplit.Length != 2)
            {
                throw new FormatException("Version supplied is not in a correct format.");
            }

            major = int.Parse(verSplit[0]);

            var minorNumber = Regex.Match(verSplit[1], @"\d+").Value;
            var releaseType = Regex.Match(verSplit[1], @"[a-zA-Z]").Value;

            if (!string.IsNullOrEmpty(minorNumber))
            {
                minor = int.Parse(minorNumber);
            }

            release = 0;

            switch (releaseType.ToLower())
            {
                case "":
                    release = 0; // Release
                    break;
                case "b":
                    release = 1; // Beta
                    break;
                default:
                    release = 2; // Alpha
                    break;
            }
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            Version v = (Version)obj;
            if (this.major > v.major)
            {
                return 1;
            }

            if (this.major < v.major)
            {
                return -1;
            }

            if (this.major == v.major)
            {
                // Compare minor version
                if (this.minor > v.minor)
                {
                    return 1;
                }

                if (this.minor < v.minor)
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string releaseType;

            switch (release)
            {
                case 0:
                    releaseType = string.Empty; // Release
                    break;
                case 1:
                    releaseType = "b"; // Beta
                    break;
                default:
                    releaseType = "a"; // Alpha
                    break;
            }

            return string.Format("{0}.{1}{2}", major, minor, releaseType);
        }
    }
}
