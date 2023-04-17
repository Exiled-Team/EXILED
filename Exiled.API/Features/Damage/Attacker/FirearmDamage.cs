// -----------------------------------------------------------------------
// <copyright file="FirearmDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for FirearmDamageHandler.
    /// </summary>
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
            Type = DamageTypeExtensions.ItemConversion.TryGetValue(Base.WeaponType, out DamageType damageType) ? damageType : DamageType.Firearm;
        }

        /// <summary>
        /// Gets the <see cref="FirearmDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new FirearmDamageHandler Base { get; }

        /// <summary>
        /// Gets the item than deal you Damage.
        /// </summary>
        public Item Item => Attacker?.CurrentItem;

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalDamage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> to give.</param>
        /// <param name="damage">The ammount of damage <see cref="float"/> to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static new FirearmDamage Create(DamageType type, float damage, Player attacker)
        {
            ItemType itemtype = DamageTypeExtensions.ItemConversion.FirstOrDefault(x => x.Value == type).Key;
            return new(new()
            {
                Damage = damage,
                Attacker = attacker.Footprint,
                WeaponType = itemtype,
                _ammoType = itemtype.GetAmmoType().GetItemType(),
            });
        }
    }
}
