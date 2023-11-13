// -----------------------------------------------------------------------
// <copyright file="AmmoType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Ammo types present in the game.
    /// </summary>
    /// <seealso cref="Extensions.ItemExtensions.GetAmmoType(ItemType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetItemType(AmmoType)"/>
    /// <seealso cref="Extensions.ItemExtensions.GetWeaponAmmoType(FirearmType)"/>
    public enum AmmoType
    {
        /// <summary>
        /// Not ammo.
        /// </summary>
        None,

        /// <summary>
        /// 5.56mm Ammunition.
        /// Used by <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        Nato556,

        /// <summary>
        /// 7.62mm Ammunition.
        /// Used by and <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Nato762,

        /// <summary>
        /// 9mm Ammunition.
        /// Used by <see cref="ItemType.GunCOM15"/>,.
        /// </summary>
        Nato9,

        /// <summary>
        /// 12 gauge shotgun ammo.
        /// Used by <see cref="ItemType.GunShotgun"/>
        /// </summary>
        Ammo12Gauge,

        /// <summary>
        /// 44 Caliber Revolver Ammo
        /// Used by <see cref="ItemType.GunRevolver"/>
        /// </summary>
        Ammo44Cal,
    }
}