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

        /// <summary>
        /// Damage dealt by Bullets.
        /// </summary>
        BulletWounds,

        /// <summary>
        /// Damage dealt by Explosions.
        /// </summary>
        Explosion,

        /// <summary>
        /// Damage dealt by Friendly Fire.
        /// </summary>
        FriendlyFire,

        /// <summary>
        /// Damage is dealth by Pocket Dimension.
        /// </summary>
        PocketDimension,

        /// <summary>
        /// Damage dealth by Recontainment.
        /// </summary>
        Recontained,

        /// <summary>
        /// Damage is dealth by Severed Hands from SCP-330.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Damage is dealth by Scp049-2
        /// </summary>
        Zombie,

        /// <summary>
        /// Damage dealth by getting crushed.
        /// </summary>
        Crushed,

        /// <summary>
        /// Damage dealth by Scp207
        /// </summary>
        Scp207,
    }
}
