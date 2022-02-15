// -----------------------------------------------------------------------
// <copyright file="ChangingItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before a player's held item changes.
    /// </summary>
    public class ChangingItemEventArgs : EventArgs
    {
        private Item newItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newItem"><inheritdoc cref="NewItem"/></param>
        public ChangingItemEventArgs(Player player, ItemBase newItem)
        {
            Player = player;
            this.newItem = Item.Get(newItem);
        }

        /// <summary>
        /// Gets the player who's changing the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the new item.
        /// </summary>
        public Item NewItem
        {
            get => newItem;
            set
            {
                if (!Player.Inventory.UserInventory.Items.TryGetValue(value.Serial, out _))
                    throw new InvalidOperationException("You cannot change ev.NewItem to an item they do not have.");

                newItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event is allowed to continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
