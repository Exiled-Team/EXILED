// -----------------------------------------------------------------------
// <copyright file="CustomReasonDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using Exiled.API.Enums;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for CustomReasonDamageHandler.
    /// </summary>
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
        /// Gets the <see cref="CustomReasonDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new CustomReasonDamageHandler Base { get; }

        /// <summary>
        /// Gets or sets DamageTypeToCustomDamage.
        /// </summary>
        public CustomDamage CustomDamage { get; set; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomReasonDamage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> to give.</param>
        /// <param name="damage">The ammount of damage <see cref="float"/> to dealt.</param>
        /// <returns>.</returns>
        public static CustomReasonDamage Create(DamageType type, float damage)
        {
            if (!CustomDamage.DamageTypeToCustomDamage.TryGetValue(type, out CustomDamage customDamage))
                customDamage = new();
            CustomReasonDamage customReasonDamagene = new(new(customDamage.DeathReason, damage, customDamage.CassieAnnouncement))
            {
                CustomDamage = customDamage,
            };
            return customReasonDamagene;
        }
    }
}
