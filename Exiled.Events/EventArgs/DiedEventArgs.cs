// -----------------------------------------------------------------------
// <copyright file="DiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
#pragma warning disable CS0618
    using System;

    using Exiled.API.Features;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all informations after a player dies.
    /// </summary>
    public class DiedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiedEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="targetOldRole">Target's old <see cref="RoleType"/>.</param>
        /// <param name="damageHandler"><inheritdoc cref="DamageHandler"/></param>
        public DiedEventArgs(Player target, RoleType targetOldRole, DamageHandlerBase damageHandler)
        {
            Killer = damageHandler is AttackerDamageHandler attackerDamageHandler ? Player.Get(attackerDamageHandler.Attacker.Hub) : null;
            Target = target;
            TargetOldRole = targetOldRole;
            Handler = new DamageHandler(target, damageHandler);
        }

        /// <summary>
        /// Gets the killer player.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the killed player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.DamageHandler"/>.
        /// </summary>
        public DamageHandler Handler { get; set; }

        /// <summary>
        /// Gets the old <see cref="RoleType"/> from the killed player.
        /// </summary>
        public RoleType TargetOldRole { get; }
    }
}
