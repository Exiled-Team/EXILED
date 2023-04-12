// -----------------------------------------------------------------------
// <copyright file="CustomReasonDamage.cs" company="Exiled Team">
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

    public class CustomReasonDamage : StandardDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="CustomReasonDamageHandler"/> class.</param>
        internal CustomReasonDamage(CustomReasonDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        internal CustomReasonDamage(DamageType type, float damage)
        {
            if (!CustomDamage.customDamage.TryGetValue(type, out CustomDamage customDamage))
                customDamage = new();

            CustomDamage = customDamage;
            Base = new(customDamage.DeathReason, damage, customDamage.CassieAnnouncement);
        }


        /// <summary>
        /// Gets the <see cref="CustomReasonDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new CustomReasonDamageHandler Base { get; }

        /// <summary>
        /// CustomDamage.
        /// </summary>
        public CustomDamage CustomDamage { get; set; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }
    }
}
