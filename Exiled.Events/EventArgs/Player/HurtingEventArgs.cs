// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.EventArgs.Interfaces;

    using CustomAttackerHandler = Exiled.API.Features.DamageHandlers.AttackerDamageHandler;
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
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="damageHandler">
        ///     <inheritdoc cref="DamageHandler" />
        /// </param>
        public HurtingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = new CustomDamageHandler(target, damageHandler);
            Player = DamageHandler.BaseIs(out CustomAttackerHandler attackerDamageHandler)
                ? attackerDamageHandler.Attacker
                : null;
            Target = target;
        }

        /// <summary>
        ///     Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => DamageHandler.Damage;
            set => DamageHandler.Damage = value;
        }

        /// <summary>
        ///     Gets the target player, who is going to be hurt.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets the <see cref="API.Features.DamageHandlers.CustomDamageHandler" /> for the event.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player will be dealt damage.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the attacker player.
        /// </summary>
        public Player Player { get; }
    }
}