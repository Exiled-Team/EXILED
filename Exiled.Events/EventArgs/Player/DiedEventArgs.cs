// -----------------------------------------------------------------------
// <copyright file="DiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.DamageHandlers;

    using Interfaces;

    using PlayerRoles;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information after a player dies.
    /// </summary>
    public class DiedEventArgs : IPlayerEvent, IAttackerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="targetOldRole">Target's old <see cref="RoleTypeId" />.</param>
        /// <param name="damageHandler">
        /// <inheritdoc cref="DamageHandler" />
        /// </param>
        public DiedEventArgs(Player target, RoleTypeId targetOldRole, DamageHandlerBase damageHandler)
        {
            DamageHandler = new CustomDamageHandler(target, damageHandler);
            Attacker = DamageHandler.BaseIs(out CustomAttackerHandler attackerDamageHandler) ? attackerDamageHandler.Attacker : null;
            Player = target;
            TargetOldRole = targetOldRole;
        }

        /// <summary>
        /// Gets the old <see cref="RoleTypeId" /> from the killed player.
        /// </summary>
        public RoleTypeId TargetOldRole { get; }

        /// <summary>
        /// Gets the dead player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="DamageHandler" />.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        /// Gets the attacker.
        /// </summary>
        public Player Attacker { get; }
    }
}