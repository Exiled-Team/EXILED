// -----------------------------------------------------------------------
// <copyright file="WarheadDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerStatsSystem;

    public class WarheadDamage : StandardDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarheadDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="WarheadDamageHandler"/> class.</param>
        internal WarheadDamage(WarheadDamageHandler damageHandler)
            : base(damageHandler) => Base = damageHandler;

        /// <summary>
        /// Gets the <see cref="WarheadDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new WarheadDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.Warhead;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarheadDamage"/> class.
        /// </summary>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <returns>.</returns>
        public static new WarheadDamage Create(float damage)
        {
            WarheadDamage warheadDamage = new(new())
            {
                Damage = damage,
            };
            return warheadDamage;
        }

    }
}
