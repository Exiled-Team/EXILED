// -----------------------------------------------------------------------
// <copyright file="DisruptorDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for DisruptorDamageHandler.
    /// </summary>
    public class DisruptorDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisruptorDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="DisruptorDamageHandler"/> class.</param>
        internal DisruptorDamage(DisruptorDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="DisruptorDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new DisruptorDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.ParticleDisruptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static DisruptorDamage Create(float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(attacker.Footprint, damage));
        }
    }
}
