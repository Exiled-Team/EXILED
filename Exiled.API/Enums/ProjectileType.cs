// -----------------------------------------------------------------------
// <copyright file="ProjectileType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Extensions;

    /// <summary>
    /// Projectile types.
    /// </summary>
    /// <seealso cref="ItemExtensions.GetItemType(ProjectileType)"/>
    /// <seealso cref="ItemExtensions.GetProjectileType(ItemType)"/>
    public enum ProjectileType
    {
        /// <summary>
        /// Not a projectile.
        /// </summary>
        None,

        /// <summary>
        /// High explosive grenade.
        /// Used by <see cref="ItemType.GrenadeHE"/>.
        /// </summary>
        FragGrenade,

        /// <summary>
        /// Flashbang.
        /// Used by <see cref="ItemType.GrenadeFlash"/>.
        /// </summary>
        Flashbang,

        /// <summary>
        /// SCP-018 ball.
        /// Used by <see cref="ItemType.SCP018"/>.
        /// </summary>
        Scp018,

        /// <summary>
        /// SCP-2176 lightbulb.
        /// Used by <see cref="ItemType.SCP2176"/>.
        /// </summary>
        Scp2176,

        /// <summary>
        /// Coal from chrithmass tree.
        /// Used by <see cref="ItemType.Coal"/>.
        /// </summary>
        Coal,

        /// <summary>
        /// Coal but Special.
        /// Used by <see cref="ItemType.SpecialCoal"/>.
        /// </summary>
        SpecialCoal,

        /// <summary>
        /// Snowball from snow.
        /// Used by <see cref="ItemType.Snowball"/>.
        /// </summary>
        Snowball,
    }
}