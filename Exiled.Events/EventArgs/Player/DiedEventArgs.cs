// -----------------------------------------------------------------------
// <copyright file="DiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Damage;
    using Exiled.API.Features.Damage.Attacker;
    using Interfaces;

    using PlayerRoles;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information after a player dies.
    /// </summary>
    public class DiedEventArgs : IPlayerEvent, IAttackerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DiedEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="targetOldRole">Target's old <see cref="RoleTypeId" />.</param>
        /// <param name="damageHandler">
        ///     <inheritdoc cref="Damage" />
        /// </param>
        public DiedEventArgs(Player target, RoleTypeId targetOldRole, DamageHandlerBase damageHandler)
        {
            Damage = DamageBase.Get(damageHandler);
            Attacker = Damage is AttackerDamage attacker ? attacker.Attacker : null;
            Player = target;
            TargetOldRole = targetOldRole;
        }

        /// <summary>
        ///     Gets the old <see cref="RoleTypeId" /> from the killed player.
        /// </summary>
        public RoleTypeId TargetOldRole { get; }

        /// <summary>
        ///     Gets the killed player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the <see cref="StandardDamage" />.
        /// </summary>
        public StandardDamage Damage { get; set; }

        /// <summary>
        ///     Gets the killer player.
        /// </summary>
        public Player Attacker { get; }
    }
}