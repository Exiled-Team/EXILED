// -----------------------------------------------------------------------
// <copyright file="PickingUpItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information before a player picks up an item.
    /// </summary>
    public class PickingUpItemEventArgs : IPlayerEvent, IPickupEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickingUpItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PickingUpItemEventArgs(Player player, ItemPickupBase pickup, bool isAllowed = true)
        {
            IsAllowed = isAllowed;
            Player = player;
            Pickup = Pickup.Get(pickup);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the item can be picked up.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the pickup that's being picked up.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        ///     Gets the player who's picking up an item.
        /// </summary>
        public Player Player { get; }
    }
}
