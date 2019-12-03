// <copyright file="PersistentToastList.cs" company="Mozilla">
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirefoxPrivateNetwork.UIUpdaters
{
    /// <summary>
    /// Represents a list of persistent toasts that require explicit dismissal from the user.
    /// </summary>
    internal class PersistentToastList : ToastCollection
    {
        private const int DefaultCapacity = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentToastList"/> class.
        /// </summary>
        public PersistentToastList() : base(DefaultCapacity)
        {
        }

        /// <summary>
        /// Remove all existing persistent toasts from the list as well as the UI.
        /// </summary>
        public void ClearAll()
        {
            foreach (UI.Components.Toast.Toast toast in ToastList)
            {
                toast.RemoveFromElement();
            }
        }

        /// <summary>
        /// Add a persistent toast to the list, reorders the toast list based on priority level, and displays when available.
        /// </summary>
        /// <param name="toast">The persistent toast to display.</param>
        public void Add(UI.Components.Toast.Toast toast)
        {
            ToastList.Add(toast);
            var newToastList = ToastList.OrderByDescending(x => x.Priority).Take(Size < Capacity ? Size : Capacity).ToList();

            // If at capacity, remove obsolete persistent toasts
            if (AtCapacity)
            {
                foreach (UI.Components.Toast.Toast existingToast in ToastList)
                {
                    if (!newToastList.Contains(existingToast))
                    {
                        existingToast.RemoveFromElement();
                    }
                }
            }

            ToastList = newToastList;
        }
    }
}
