// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Runtime.Remoting.Messaging;

    using Exiled.API.Features;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all information before a player gets damaged.
    /// </summary>
    public class HurtingEventArgs : EventArgs
    {
        private DamageHandlerBase damageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="damageHandler"><inheritdoc cref="DamageHandler"/></param>
        public HurtingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            Attacker = damageHandler is AttackerDamageHandler attackerDamageHandler
                ? Player.Get(attackerDamageHandler.Attacker.Hub)
                : null;
            Target = target;
            DamageHandler = damageHandler;
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
        /// Gets the hit informations.
        /// </summary>
        public DamageHandlerBase DamageHandler
        {
            get => damageHandler;
            private set => damageHandler = value;
        }

        /// <summary>
        /// Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => damageHandler is StandardDamageHandler standardDamageHandler ? standardDamageHandler.Damage : 0.0f;
            set
            {
                if (damageHandler is StandardDamageHandler standardDamageHandler)
                    standardDamageHandler.Damage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player will be dealt damage.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
