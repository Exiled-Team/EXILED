// -----------------------------------------------------------------------
// <copyright file="ItemAddedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information after adding an item to a player's inventory.
    /// </summary>
    public class ItemAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAddedEventArgs"/> class.
        /// </summary>
        /// <param name="inventory">The <see cref="Inventory"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> the <see cref="ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public ItemAddedEventArgs(Inventory inventory, ItemBase itemBase, ItemPickupBase pickupBase)
        {
            Player = Player.Get(inventory._hub);
            Item = Item.Get(itemBase);
            Pickup = Pickup.Get(pickupBase);
        }

        /// <summary>
        /// Gets the player that had the item added.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item that was added.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the pickup that the item originated from or <see langword="null"/> if the item was not picked up.
        /// </summary>
        public Pickup Pickup { get; }
    }
}
