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

        public CustomDamage()
        {
            DamageType = DamageType.Custom;
            DeathReason = "Custom";
            CassieAnnouncement = string.Empty;
        }

        public CustomDamage(DamageType damageType, string deathReason, string cassieAnnouncement)
        {
            DamageType = damageType;
            DeathReason = deathReason;
            CassieAnnouncement = cassieAnnouncement;
            customDamage.Add(damageType, this);
        }

        /// <summary>
        /// .
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string DeathReason { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public string CassieAnnouncement { get; set; }

    }
}
