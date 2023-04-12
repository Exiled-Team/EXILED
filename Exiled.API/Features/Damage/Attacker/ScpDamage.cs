// -----------------------------------------------------------------------
// <copyright file="ScpDamage.cs" company="Exiled Team">
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

    public class ScpDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="ScpDamageHandler"/> class.</param>
        public ScpDamage(ScpDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="ScpDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new ScpDamageHandler Base { get; }

    }
}
