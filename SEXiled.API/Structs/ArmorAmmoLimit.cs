// -----------------------------------------------------------------------
// <copyright file="ArmorAmmoLimit.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Structs
{
    using SEXiled.API.Enums;

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
    }
}
