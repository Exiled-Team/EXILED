// -----------------------------------------------------------------------
// <copyright file="WarheadDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Enums;
    using PlayerStatsSystem;

    public class WarheadDamage : StandardDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarheadDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="WarheadDamageHandler"/> class.</param>
        public WarheadDamage(WarheadDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="WarheadDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new WarheadDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; } = DamageType.Warhead;
    }
}
