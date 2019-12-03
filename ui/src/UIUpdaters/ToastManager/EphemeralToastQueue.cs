// <copyright file="EphemeralToastQueue.cs" company="Mozilla">
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
    /// Represents a queue of ephemeral toasts that will be dismissed upon an elapsed duration period.
    /// </summary>
    internal class EphemeralToastQueue : ToastCollection
    {
        private const int DefaultCapacity = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="EphemeralToastQueue"/> class.
        /// </summary>
        public EphemeralToastQueue() : base(DefaultCapacity)
        {
        }

        /// <summary>
        /// Gets or sets the current ephemeral toast being displayed.
        /// </summary>
        public UI.Components.Toast.Toast DisplayToast { get; set; }

        /// <summary>
        /// Removes the next available ephemeral toast from the queue and signifies the the toast scheduler to display it.
        /// </summary>
        /// <returns>The dequeued ephemeral toast to be displayed.</returns>
        public UI.Components.Toast.Toast Dequeue()
        {
            var toast = ToastList.FirstOrDefault();
            if (toast != null)
            {
                ToastList.RemoveAt(0);
            }

            DisplayToast = toast;
            return toast;
        }

        /// <summary>
        /// Adds an ephemeral toast to the queue and reorders the queue based on priority level.
        /// </summary>
        /// <param name="toast">The ephemeral toast to add to the queue.</param>
        public void Add(UI.Components.Toast.Toast toast)
        {
            ToastList.Add(toast);
            ToastList = ToastList.OrderByDescending(x => x.Priority).Take(Size < Capacity ? Size : Capacity).ToList();
        }

        /// <summary>
        /// Refreshes the text content of all the toasts in the ephemeral toast queue, as well as the current ephemeral toast being displayed.
        /// </summary>
        public override void RefreshAllContent()
        {
            base.RefreshAllContent();

            if (DisplayToast != null)
            {
                DisplayToast.RefreshContent();
            }
        }
    }
}
