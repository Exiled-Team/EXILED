// -----------------------------------------------------------------------
// <copyright file="DiedEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.DamageHandlers;

    using AttackerDamageHandler = PlayerStatsSystem.AttackerDamageHandler;
    using CustomAttackerHandler = SEXiled.API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

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
            Handler = new CustomDamageHandler(target, damageHandler);
            Killer = Handler.BaseIs(out CustomAttackerHandler attackerDamageHandler) ? attackerDamageHandler.Attacker : null;
            Target = target;
            TargetOldRole = targetOldRole;
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
        /// Gets or sets the <see cref="DamageHandler"/>.
        /// </summary>
        public CustomDamageHandler Handler { get; set; }

        /// <summary>
        /// Gets the old <see cref="RoleType"/> from the killed player.
        /// </summary>
        public RoleType TargetOldRole { get; }
    }
}
