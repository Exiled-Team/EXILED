// -----------------------------------------------------------------------
// <copyright file="PickingUpScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    ///     Contains all information before a player picks up an SCP-330.
    /// </summary>
    public class PickingUpScp330EventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickingUpScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        public PickingUpScp330EventArgs(Player player, Scp330Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
        }

        /// <summary>
        ///     Gets or sets a value representing the <see cref="Scp330Pickup" /> being picked up.
        /// </summary>
        public Scp330Pickup Pickup { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }
    }
}