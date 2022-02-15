// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="SEXiled Team">
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
    /// Contains all information before a player gets damaged.
    /// </summary>
    public class HurtingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="damageHandler"><inheritdoc cref="DamageHandler"/></param>
        public HurtingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            Handler = new CustomDamageHandler(target, damageHandler);
            Attacker = Handler.BaseIs(out CustomAttackerHandler attackerDamageHandler)
                ? attackerDamageHandler.Attacker
                : null;
            Target = target;
        }

        /// <summary>
        /// Gets the attacker player.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        /// Gets the target player, who is going to be hurt.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomDamageHandler"/> for the event.
        /// </summary>
        public CustomDamageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => Handler.Damage;
            set => Handler.Damage = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player will be dealt damage.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
