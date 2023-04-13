// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="Exiled Team">
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

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information before a player gets damaged.
    /// </summary>
    public class HurtingEventArgs : IPlayerEvent, IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HurtingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        ///     <inheritdoc cref="DamageHandler" />
        /// </param>
        public HurtingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = DamageBase.Get(damageHandler);

            Attacker = DamageHandler is AttackerDamage attacker ? attacker.Attacker : null;
            Player = target;
        }

        /// <summary>
        ///     Gets the target player, who is going to be hurt.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the attacker player.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        ///     Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => DamageHandler.Damage;
            set => DamageHandler.Damage = value;
        }

        /// <summary>
        ///     Gets or sets the <see cref="DamageBase" /> for the event.
        /// </summary>
        public StandardDamage DamageHandler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player will be dealt damage.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}