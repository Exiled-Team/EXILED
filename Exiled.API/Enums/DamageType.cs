// -----------------------------------------------------------------------
// <copyright file="DamageType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Identifiers for types of damage.
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// Unknown damage source.
        /// </summary>
        Unknown,

        /// <summary>
        /// Fall damage.
        /// </summary>
        Falldown,

        /// <summary>
        /// Alpha Warhead.
        /// </summary>
        Warhead,

        /// <summary>
        /// LCZ Decontamination.
        /// </summary>
        Decontamination,

        /// <summary>
        /// <see cref="EffectType.Asphyxiated"/>.
        /// </summary>
        Asphyxiation,

        /// <summary>
        /// <see cref="EffectType.Poisoned"/>.
        /// </summary>
        Poison,

        /// <summary>
        /// <see cref="EffectType.Bleeding"/>.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Damage dealt by a <see cref="API.Features.Items.Firearm"/>.
        /// </summary>
        Firearm,

        /// <summary>
        /// Damage dealt by a <see cref="API.Features.Items.MicroHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Damage dealt by a Tesla Gate.
        /// </summary>
        Tesla,

        /// <summary>
        /// Damage is dealt by an SCP.
        /// </summary>
        Scp,
    }
}
