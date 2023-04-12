// -----------------------------------------------------------------------
// <copyright file="FirearmDamage.cs" company="Exiled Team">
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
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using PlayerStatsSystem;

    public class FirearmDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="FirearmDamageHandler"/> class.</param>
        internal FirearmDamage(FirearmDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
            Type = DamageTypeExtensions.ItemConversion[Base.WeaponType];
        }

        /// <summary>
        /// Gets the <see cref="FirearmDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new FirearmDamageHandler Base { get; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Item Item => Attacker?.CurrentItem;

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }
    }
}
