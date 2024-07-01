// -----------------------------------------------------------------------
// <copyright file="HurtEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.DamageHandlers;

    using Interfaces;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information before a player gets damaged.
    /// </summary>
    public class HurtEventArgs : IAttackerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HurtEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        /// <inheritdoc cref="DamageHandler" />
        /// </param>
        /// <param name="handlerOutput">
        /// <inheritdoc cref="HandlerOutput" />
        /// </param>
        public HurtEventArgs(Player target, DamageHandlerBase damageHandler, DamageHandlerBase.HandlerOutput handlerOutput)
        {
            DamageHandler = new CustomDamageHandler(target, damageHandler);
            Attacker = DamageHandler.BaseIs(out CustomAttackerHandler attackerDamageHandler) ? attackerDamageHandler.Attacker : null;
            Player = target;
            HandlerOutput = handlerOutput;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Player Attacker { get; }

        /// <summary>
        /// Gets the amount of inflicted damage.
        /// </summary>
        public float Amount => DamageHandler.Damage;

        /// <summary>
        /// Gets or sets the action than will be made on the player.
        /// </summary>
        public DamageHandlerBase.HandlerOutput HandlerOutput { get; set; }

        /// <inheritdoc/>
        public CustomDamageHandler DamageHandler { get; set; }
    }
}