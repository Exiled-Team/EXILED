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
        /// Damage dealt by SCP-096.
        /// </summary>
        Scp096,

        /// Damage dealt by frag grenades.
        /// </summary>
        Explosion,

        /// <summary>
        /// Damage dealt by SCP-018.
        /// </summary>
        Scp018,

        /// <summary>
        /// Damage dealt by SCP-207.
        /// </summary>
        Scp207,

        /// <summary>
        /// Damage is dealt by SCP Recontainment procedure.
        /// </summary>
        Recontainment,

        /// <summary>
        /// Crushed by the checkpoint killer trigger.
        /// </summary>
        Crushed,

        /// <summary>
        /// Damage caused by the femur breaker.
        /// </summary>
        FemurBreaker,

        /// <summary>
        /// Damage caused by the pocket dimension.
        /// </summary>
        PocketDimension,

        /// <summary>
        /// Damage caused by the friendly fire detector.
        /// </summary>
        FriendlyFireDetector,

        /// <summary>
        /// Damage caused by severed hands.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Damage cause by CustomDamageHandler plugin.
        /// </summary>
        CustomDamage,
    }
}
