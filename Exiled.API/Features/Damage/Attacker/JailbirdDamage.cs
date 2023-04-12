// -----------------------------------------------------------------------
// <copyright file="JailbirdDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerStatsSystem;
    using UnityEngine;

    public class JailbirdDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="JailbirdDamageHandler"/> class.</param>
        internal JailbirdDamage(JailbirdDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="JailbirdDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new JailbirdDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.Jailbird;

        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdDamage"/> class.
        /// </summary>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static JailbirdDamage Create(float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(attacker.ReferenceHub, damage, Vector3.zero));
        }
    }
}
