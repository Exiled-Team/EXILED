// -----------------------------------------------------------------------
// <copyright file="JailbirdDamage.cs" company="Exiled Team">
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

    public class JailbirdDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="JailbirdDamageHandler"/> class.</param>
        public JailbirdDamage(JailbirdDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="JailbirdDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new JailbirdDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; } = DamageType.Jailbird;
    }
}
