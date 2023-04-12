// -----------------------------------------------------------------------
// <copyright file="Scp049Damage.cs" company="Exiled Team">
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
        }

        /// <summary>
        /// Gets the <see cref="Scp049DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp049DamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; } = DamageType.Scp049;

    }
}
