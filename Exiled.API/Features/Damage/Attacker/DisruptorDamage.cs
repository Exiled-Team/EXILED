// -----------------------------------------------------------------------
// <copyright file="DisruptorDamage.cs" company="Exiled Team">
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

    public class DisruptorDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisruptorDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="DisruptorDamageHandler"/> class.</param>
        internal DisruptorDamage(DisruptorDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="DisruptorDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new DisruptorDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; } = DamageType.ParticleDisruptor;

    }
}
