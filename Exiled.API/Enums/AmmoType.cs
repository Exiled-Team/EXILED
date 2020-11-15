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
    public enum AmmoType
    {
        /// <summary>
        /// 5.56mm Ammunition.
        /// Used by <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        Nato556,

        /// <summary>
        /// 7.62mm Ammunition.
        /// Used by <see cref="ItemType.GunMP7"/> and <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Nato762,

        /// <summary>
        /// 9mm Ammunition.
        /// Used by <see cref="ItemType.GunCOM15"/>, <see cref="ItemType.GunProject90"/> and <see cref="ItemType.GunUSP"/>.
        /// </summary>
        Nato9,
    }
}
