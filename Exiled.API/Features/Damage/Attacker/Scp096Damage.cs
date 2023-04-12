// -----------------------------------------------------------------------
// <copyright file="Scp096Damage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

    public class Scp096Damage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Damage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="Scp096DamageHandler"/> class.</param>
        internal Scp096Damage(Scp096DamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
            Type = Base._attackType switch
            {
                Scp096DamageHandler.AttackType.GateKill => DamageType.Scp096Gate,
                Scp096DamageHandler.AttackType.SlapLeft => DamageType.Scp096SlapLeft,
                Scp096DamageHandler.AttackType.SlapRight => DamageType.Scp096SlapRight,
                Scp096DamageHandler.AttackType.Charge => DamageType.Scp096Charge,
                _ => DamageType.Unknown,
            };
        }

        /// <summary>
        /// Gets the <see cref="Scp096DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp096DamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }
    }
}
