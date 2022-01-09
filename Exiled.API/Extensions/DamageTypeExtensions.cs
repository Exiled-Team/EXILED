// -----------------------------------------------------------------------
// <copyright file="DamageTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="DamageType"/>.
    /// </summary>
    public static class DamageTypeExtensions
    {
        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by weapon.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkMicro">Indicates whether the MicroHid damage type should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by weapon or not.</returns>
        public static bool IsWeapon(this DamageType type, bool checkMicro = true)
            => type == DamageType.Crossvec || type == DamageType.Logicer ||
               type == DamageType.Revolver || type == DamageType.Shotgun ||
               type == DamageType.AK || type == DamageType.Com15 || type == DamageType.Com18 ||
               type == DamageType.E11Sr || type == DamageType.Fsp9 || (checkMicro && type == DamageType.MicroHid);

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by SCP.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP or not.</returns>
        public static bool IsScp(this DamageType type)
            => type == DamageType.Scp || type == DamageType.Scp049 ||
               type == DamageType.Scp096 || type == DamageType.Scp106 ||
               type == DamageType.Scp173 || type == DamageType.Scp939 || type == DamageType.Scp0492;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by status effect.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by status effect or not.</returns>
        public static bool IsStatusEffect(this DamageType type)
            => type == DamageType.Asphyxiation || type == DamageType.Poison || type == DamageType.Bleeding;
    }
}
