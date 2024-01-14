// -----------------------------------------------------------------------
// <copyright file="RemovingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before removing an item from a player's inventory.
    /// </summary>
    public class RemovingItemEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public RemovingItemEventArgs(Player player, Item item, Pickup pickup, bool isAllowed = true)
        {
            Player = player;
            Item = item;
            Pickup = pickup;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the item to be removed.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the pickup relative to the item.
        /// </summary>
        public Pickup Pickup { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}