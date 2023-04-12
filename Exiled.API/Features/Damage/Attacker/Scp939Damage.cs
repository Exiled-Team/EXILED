// -----------------------------------------------------------------------
// <copyright file="Scp096Damage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Enums;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

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
    }
}
