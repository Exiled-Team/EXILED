// -----------------------------------------------------------------------
// <copyright file="Scp939Damage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    /// A wrapper class for Scp939DamageHandler.
    /// </summary>
    public class Scp939Damage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp939Damage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="Scp939DamageHandler"/> class.</param>
        internal Scp939Damage(Scp939DamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
            Type = Base._damageType switch
            {
                Scp939DamageType.Claw => DamageType.Scp939Claw,
                Scp939DamageType.LungeTarget => DamageType.Scp939LungeTarget,
                Scp939DamageType.LungeSecondary => DamageType.Scp939LungeSecondary,
                Scp939DamageType.None or _ => DamageType.Unknown,
            };
        }

        /// <summary>
        /// Gets the <see cref="Scp939DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp939DamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScpDamage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> to give.</param>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static new Scp939Damage Create(DamageType type, float damage, Player attacker)
        {
            attacker ??= Server.Host;
            return new(new(null, type switch
            {
                DamageType.Scp939Claw => Scp939DamageType.Claw,
                DamageType.Scp939LungeSecondary => Scp939DamageType.LungeSecondary,
                DamageType.Scp939LungeTarget => Scp939DamageType.LungeTarget,
                _ => Scp939DamageType.None,
            })
            {
                Damage = damage,
                Attacker = attacker.Footprint,
                _hitPos = attacker.RelativePosition,
            });
        }
    }
}
