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

    using PlayerStatsSystem;

    public class Scp096Damage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Damage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="Scp096DamageHandler"/> class.</param>
        public Scp096Damage(Scp096DamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="Scp096DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp096DamageHandler Base { get; }

    }
}
