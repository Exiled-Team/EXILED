// -----------------------------------------------------------------------
// <copyright file="DamagingScp244EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using InventorySystem.Items.Usables.Scp244;

    using PlayerStatsSystem;

    using AttackerDamageHandler = PlayerStatsSystem.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information before damage is dealt to a <see cref="Scp244DeployablePickup"/>.
    /// </summary>
    public class DamagingScp244EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamagingScp244EventArgs"/> class.
        /// </summary>
        /// <param name="scp244"><inheritdoc cref="Scp244"/></param>
        /// <param name="damage">The damage being dealt.</param>
        /// <param name="handler"><inheritdoc cref="Handler"/></param>
        public DamagingScp244EventArgs(Scp244DeployablePickup scp244, float damage, DamageHandlerBase handler)
        {
            IsAllowed = handler is ExplosionDamageHandler;
            Scp244 = (Scp244Pickup)Pickup.Get(scp244);
            Handler = new(handler is AttackerDamageHandler attackerDamageHandler ? Player.Get(attackerDamageHandler.Attacker.Hub) : null, handler);
            Handler.Damage = damage;
        }

        /// <summary>
        /// Gets the <see cref="Scp244Pickup"/> object that is damaged.
        /// </summary>
        public Scp244Pickup Scp244 { get; }

        /// <summary>
        /// Gets the Damage handler for this event.
        /// </summary>
        public DamageHandler Handler { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be broken.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
