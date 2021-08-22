// -----------------------------------------------------------------------
// <copyright file="ItemDroppedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information after a player drops an item.
    /// </summary>
    public class ItemDroppedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDroppedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        public ItemDroppedEventArgs(Player player, ItemPickupBase pickup)
        {
            Player = player;
            Pickup = Pickup.Get(pickup);
        }

        /// <summary>
        /// Gets the player who dropped the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the dropped pickup.
        /// </summary>
        public Pickup Pickup { get; }
    }
}
