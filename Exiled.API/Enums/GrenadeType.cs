// -----------------------------------------------------------------------
// <copyright file="GrenadeType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// The unique type of grenade.
    /// </summary>
    public enum GrenadeType
    {
        /// <summary>
        /// Frag grenade.
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
    }
}
