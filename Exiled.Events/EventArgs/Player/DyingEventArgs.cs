// -----------------------------------------------------------------------
// <copyright file="DyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using Exiled.API.Features.Damage;
    using Exiled.API.Features.Damage.Attacker;
    using Interfaces;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information before a player dies.
    /// </summary>
    public class DyingEventArgs : IPlayerEvent, IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DyingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        ///     <inheritdoc cref="Damage" />
        /// </param>
        public DyingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            Damage = DamageBase.Get(damageHandler);
            ItemsToDrop = new List<Item>(target?.Items?.ToList() ?? new());
            Attacker = Damage is AttackerDamage attacker ? attacker.Attacker : null;
            Player = target;
        }

        /// <summary>
        ///     Gets or sets the list of items to be dropped.
        /// </summary>
        public List<Item> ItemsToDrop { get; set; }

        /// <summary>
        ///     Gets the dying player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the <see cref="StandardDamage" />.
        /// </summary>
        public StandardDamage Damage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can be killed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the killing player.
        /// </summary>
        public Player Attacker { get; }
    }
}