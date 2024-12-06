// -----------------------------------------------------------------------
// <copyright file="AddingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Interfaces;
    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before adding an item to a player's inventory.
    /// </summary>
    public class AddingItemEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="addReason"><inheritdoc cref="AddReason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AddingItemEventArgs(Player player, Item item, Pickup pickup, ItemAddReason addReason, bool isAllowed = true)
        {
            Player = player;
            Item = item;
            Pickup = pickup;
            AddReason = addReason;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the item to be added.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the pickup that the item originated from or <see langword="null"/> if the item was not picked up.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets the reason of why the item was added to the player inventory.
        /// </summary>
        public ItemAddReason AddReason { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}