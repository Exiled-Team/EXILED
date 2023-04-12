// -----------------------------------------------------------------------
// <copyright file="CustomDamage.cs" company="Exiled Team">
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

    public class CustomDamage
    {
        internal static Dictionary<DamageType, CustomDamage> customDamage = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamage"/> class.
        /// </summary>
        public CustomDamage()
        {
            DamageType = DamageType.Custom;
            DeathReason = "Custom";
            CassieAnnouncement = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamage"/> class.
        /// </summary>
        /// <param name="damageType">.</param>
        /// <param name="damageName">..</param>
        /// <param name="deathReason">...</param>
        /// <param name="cassieAnnouncement">....</param>
        public CustomDamage(DamageType damageType, string damageName, string deathReason, string cassieAnnouncement = "")
        {
            DamageType = damageType;
            DamageName = damageName;
            DeathReason = deathReason;
            CassieAnnouncement = cassieAnnouncement;
            customDamage.Add(damageType, this);
        }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public string DamageName { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public string DeathReason { get; set; }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public string CassieAnnouncement { get; set; }

    }
}
