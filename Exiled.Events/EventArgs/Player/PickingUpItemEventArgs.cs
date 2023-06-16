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

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Pickup Pickup { get; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}