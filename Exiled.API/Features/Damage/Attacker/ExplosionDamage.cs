// -----------------------------------------------------------------------
// <copyright file="ExplosionDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerStatsSystem;
    using UnityEngine;

    public class ExplosionDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosionDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="ExplosionDamageHandler"/> class.</param>
        internal ExplosionDamage(ExplosionDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="ExplosionDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new ExplosionDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.Explosion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static new ExplosionDamage Create(float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(attacker.Footprint, Vector3.zero, damage, 50));
        }

    }
}
