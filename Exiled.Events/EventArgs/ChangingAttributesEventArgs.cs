// -----------------------------------------------------------------------
// <copyright file="ChangingAttributesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features.Items;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all informations before changing item attributes.
    /// </summary>
    public class ChangingAttributesEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttributesEventArgs"/> class.
        /// </summary>
        /// <param name="oldItem"><inheritdoc cref="OldItem"/></param>
        /// <param name="newItem"><inheritdoc cref="NewItem"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAttributesEventArgs(ItemBase oldItem, ItemBase newItem, bool isAllowed = true) {
            OldItem = Item.Get(oldItem);
            NewItem = Item.Get(newItem);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the old item.
        /// </summary>
        public Item OldItem { get; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public Item NewItem { get; set; }

        /// <summary>
        /// Gets the new item unique id.
        /// </summary>
        public int NewUniqueId => NewItem.Serial;

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
