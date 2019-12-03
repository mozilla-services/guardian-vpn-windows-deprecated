// <copyright file="VpnNotUnprotectedConverter.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FirefoxPrivateNetwork.UI.Components
{
    /// <summary>
    /// Converter that returns true whenever the connection status is not unprotected (protected, connecting, disconnecting).
    /// </summary>
    public class VpnNotUnprotectedConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Models.ConnectionState)value != Models.ConnectionState.Unprotected;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
