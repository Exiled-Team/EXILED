// -----------------------------------------------------------------------
// <copyright file="ChangingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before a player's held item changes.
    /// </summary>
    public class ChangingItemEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        private Item newItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newItem">
        /// <inheritdoc cref="Item" />
        /// </param>
        public ChangingItemEventArgs(Player player, ItemBase newItem)
        {
            Player = player;
            this.newItem = Item.Get(newItem);
        }

        /// <summary>
        /// Gets or sets the new item.
        /// </summary>
        public Item Item
        {
            get => newItem;
            set
            {
                if (!Player.Inventory.UserInventory.Items.TryGetValue(value.Serial, out _))
                    throw new InvalidOperationException("ev.NewItem cannot be set to an item they do not have.");

                newItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event is allowed to continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who's changing the item.
        /// </summary>
        public Player Player { get; }
    }
}