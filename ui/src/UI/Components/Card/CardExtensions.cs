// <copyright file="CardExtensions.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Windows;

namespace FirefoxPrivateNetwork.UI
{
    /// <summary>
    /// Dependency properties for the <see cref="Components.Card"/> component.
    /// </summary>
    internal class CardExtensions : DependencyObject
    {
        /// <summary>
        /// Dependency property for the VPN connection status.
        /// </summary>
        public static readonly DependencyProperty VpnStatusProperty = DependencyProperty.RegisterAttached("VpnStatus", typeof(Models.ConnectionState), typeof(CardExtensions));

        /// <summary>
        /// Dependency property for the VPN connection stability.
        /// </summary>
        public static readonly DependencyProperty VpnStabilityProperty = DependencyProperty.RegisterAttached("VpnStability", typeof(Models.ConnectionStability), typeof(CardExtensions));

        /// <summary>
        /// Getter for <see cref="VpnStatusProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static Models.ConnectionState GetVpnStatus(DependencyObject d)
        {
            return (Models.ConnectionState)d.GetValue(VpnStatusProperty);
        }

        /// <summary>
        /// Setter for <see cref="VpnStatusProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetVpnStatus(DependencyObject d, Models.ConnectionState value)
        {
            d.SetValue(VpnStatusProperty, value);
        }

        /// <summary>
        /// Getter for <see cref="VpnStabilityProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <returns><see cref="DependencyProperty"/> value.</returns>
        public static Models.ConnectionStability GetVpnStability(DependencyObject d)
        {
            return (Models.ConnectionStability)d.GetValue(VpnStabilityProperty);
        }

        /// <summary>
        /// Setter for <see cref="VpnStabilityProperty"/>.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject"/>.</param>
        /// <param name="value"><see cref="DependencyProperty"/> value.</param>
        public static void SetVpnStability(DependencyObject d, Models.ConnectionStability value)
        {
            d.SetValue(VpnStabilityProperty, value);
        }
    }
}
