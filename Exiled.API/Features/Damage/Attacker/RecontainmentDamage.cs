// -----------------------------------------------------------------------
// <copyright file="RecontainmentDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for RecontainmentDamageHandler.
    /// </summary>
    public class RecontainmentDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecontainmentDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="RecontainmentDamageHandler"/> class.</param>
        internal RecontainmentDamage(RecontainmentDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="RecontainmentDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new RecontainmentDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.Recontainment;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecontainmentDamage"/> class.
        /// </summary>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static RecontainmentDamage Create(float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(attacker.Footprint))
            {
                Damage = damage,
            };
        }
    }
}
