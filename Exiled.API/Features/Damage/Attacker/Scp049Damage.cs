// -----------------------------------------------------------------------
// <copyright file="Scp049Damage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using System;

    using Exiled.API.Enums;
    using PlayerStatsSystem;

    public class Scp049Damage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp049Damage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="Scp049DamageHandler"/> class.</param>
        internal Scp049Damage(Scp049DamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
            Type = Base.DamageSubType switch
            {
                Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                Scp049DamageHandler.AttackType.Instakill => DamageType.Scp049,
                Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                _ => DamageType.Unknown,
            };
        }

        /// <summary>
        /// Gets the <see cref="Scp049DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp049DamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> to give.</param>
        /// <param name="damage">The ammount of damage <see cref="float"/> to dealt.</param>
        /// <returns>.</returns>
        public static Scp049Damage Create(DamageType type, float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(attacker.Footprint, damage, type switch
            {
                DamageType.Scp049 => Scp049DamageHandler.AttackType.Instakill,
                DamageType.Scp0492 => Scp049DamageHandler.AttackType.Scp0492,
                DamageType.CardiacArrest => Scp049DamageHandler.AttackType.CardiacArrest,
                _ => throw new NotImplementedException(),
            }));
        }
    }
}
