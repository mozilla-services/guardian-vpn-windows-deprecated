// <copyright file="ToastCollection.cs" company="Mozilla">
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
    /// Represents a collection of in-app toasts.
    /// </summary>
    internal class ToastCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToastCollection"/> class.
        /// </summary>
        /// <param name="capacity">Maximum number of toasts in the collection.</param>
        public ToastCollection(int capacity)
        {
            this.Capacity = capacity;
        }

        /// <summary>
        /// Gets or sets the stored list of in-app toasts.
        /// </summary>
        public List<UI.Components.Toast.Toast> ToastList { get; set; } = new List<UI.Components.Toast.Toast>();

        /// <summary>
        /// Gets or sets the maximum number of toasts in the collection.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets a value indicating whether the toast collection is at capacity or not.
        /// </summary>
        public bool AtCapacity
        {
            get
            {
                return Size >= Capacity;
            }
        }

        /// <summary>
        /// Gets the current number of toasts in the collection.
        /// </summary>
        public int Size => ToastList.Count();

        /// <summary>
        /// Refreshes the text content of all the toasts in the collection.
        /// </summary>
        public virtual void RefreshAllContent()
        {
            ToastList.ForEach(toast => toast.RefreshContent());
        }
    }
}
