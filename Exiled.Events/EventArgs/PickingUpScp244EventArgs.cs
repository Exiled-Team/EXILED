// -----------------------------------------------------------------------
// <copyright file="PickingUpScp244EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    /// Contains all information before a player picks up an SCP-244.
    /// </summary>
    public class PickingUpScp244EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpScp244EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp244"><inheritdoc cref="Scp244"/></param>
        public PickingUpScp244EventArgs(Player player, Scp244DeployablePickup scp244)
        {
            Player = player;
            Scp244 = scp244;
            Pickup = Pickup.Get(scp244);
        }

        /// <summary>
        /// Gets the player who's interacting with SCP-244.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value representing the <see cref="API.Features.Items.Pickup"/> being picked up.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets a value representing the <see cref="Scp244DeployablePickup"/> being picked up.
        /// </summary>
        public Scp244DeployablePickup Scp244 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
