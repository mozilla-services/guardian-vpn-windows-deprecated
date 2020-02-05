// <copyright file="RangeConverter.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Windows.Data;

namespace FirefoxPrivateNetwork.FxA
{
    /// <summary>
    /// Determines if there is at least one device.
    /// </summary>
    internal class RangeConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int numDevices = (int)value;

            if (numDevices > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
