// -----------------------------------------------------------------------
// <copyright file="DroppedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Pickups;
    using Interfaces;

    /// <summary>
    /// Contains all information after a player drops an item.
    /// </summary>
    public class DroppedItemEventArgs : IPlayerEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="pickup">
        /// <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="wasThrown">
        /// <inheritdoc cref="WasThrown" />
        /// </param>
        public DroppedItemEventArgs(Player player, Pickup pickup, bool wasThrown)
        {
            Player = player;
            Pickup = pickup;
            WasThrown = wasThrown;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup was thrown.
        /// </summary>
        public bool WasThrown { get; set; }

        /// <inheritdoc/>
        public Pickup Pickup { get; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}