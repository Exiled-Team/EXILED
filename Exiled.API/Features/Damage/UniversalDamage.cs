// -----------------------------------------------------------------------
// <copyright file="UniversalDamage.cs" company="Exiled Team">
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

    using PlayerStatsSystem;

    public class UniversalDamage : StandardDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="UniversalDamageHandler"/> class.</param>
        public UniversalDamage(UniversalDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="UniversalDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new UniversalDamageHandler Base { get; }

    }
}
