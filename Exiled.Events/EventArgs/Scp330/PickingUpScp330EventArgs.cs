// -----------------------------------------------------------------------
// <copyright file="PickingUpScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Interfaces.Pickup;

    using BaseScp330 = InventorySystem.Items.Usables.Scp330.Scp330Pickup;
    using Scp330Pickup = Exiled.API.Features.Pickups.Scp330Pickup;

    /// <summary>
    ///     Contains all information before a player picks up an SCP-330.
    /// </summary>
    public class PickingUpScp330EventArgs : IPlayerEvent, IDeniableEvent, IPickupScp330Event
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
        public PickingUpScp330EventArgs(Player player, BaseScp330 pickup)
        {
            Player = player;
            Scp330 = (Scp330Pickup)Pickup.Get(pickup);
        }

        /// <summary>
        ///     Gets or sets a value representing the <see cref="Scp330Pickup" /> being picked up.
        /// </summary>
        public Scp330Pickup Scp330 { get; set; }

        /// <summary>
        ///     Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
