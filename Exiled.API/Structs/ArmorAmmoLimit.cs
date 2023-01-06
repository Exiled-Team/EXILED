// -----------------------------------------------------------------------
// <copyright file="ArmorAmmoLimit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using Enums;

    using Extensions;

    using InventorySystem.Items.Armor;

    /// <summary>
    /// The limit of a certain <see cref="Enums.AmmoType"/> when wearing a piece of armor.
    /// </summary>
    public struct ArmorAmmoLimit
    {
        /// <summary>
        /// The <see cref="Enums.AmmoType"/> being limited.
        /// </summary>
        public AmmoType AmmoType;

        /// <summary>
        /// The amount to limit to.
        /// </summary>
        public ushort Limit;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorAmmoLimit"/> struct.
        /// </summary>
        /// <param name="type">The <see cref="Enums.AmmoType"/> of the ammo.</param>
        /// <param name="limit">The ammo limit.</param>
        public ArmorAmmoLimit(AmmoType type, ushort limit)
        {
            AmmoType = type;
            Limit = limit;
        }

        /// <summary>
        /// Converts a base game <see cref="BodyArmor.ArmorAmmoLimit"/> to its appropriate <see cref="ArmorAmmoLimit"/>.
        /// </summary>
        /// <param name="armorLimit">Base game armor limit.</param>
        public static implicit operator ArmorAmmoLimit(BodyArmor.ArmorAmmoLimit armorLimit) =>
            new(armorLimit.AmmoType.GetAmmoType(), armorLimit.Limit);

        /// <summary>
        /// Converts a <see cref="ArmorAmmoLimit"/> to its appropriate base game <see cref="BodyArmor.ArmorAmmoLimit"/>.
        /// </summary>
        /// <param name="armorLimit">armor limit.</param>
        public static explicit operator BodyArmor.ArmorAmmoLimit(ArmorAmmoLimit armorLimit) =>
            new()
                { AmmoType = armorLimit.AmmoType.GetItemType(), Limit = armorLimit.Limit };
    }
}