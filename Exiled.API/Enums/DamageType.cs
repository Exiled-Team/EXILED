// -----------------------------------------------------------------------
// <copyright file="DamageType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

    /// <summary>
    /// Identifiers for types of damage.
    /// </summary>
    /// <seealso cref="Player.Hurt(float, DamageType, string)"/>
    /// <seealso cref="Player.Hurt(Player, float, DamageType, Features.DamageHandlers.DamageHandlerBase.CassieAnnouncement)"/>
    /// <seealso cref="Player.Hurt(Player, float, DamageType, Features.DamageHandlers.DamageHandlerBase.CassieAnnouncement, string)"/>
    /// <seealso cref="Player.Kill(DamageType, string)"/>
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
        /// Create dealt by a <see cref="Features.Items.Firearm"/> when the <see cref="ItemType"/> used is not available.
        /// </summary>
        Firearm,

        /// <summary>
        /// Create dealt by a <see cref="Features.Items.MicroHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Create dealt by a Tesla Gate.
        /// </summary>
        Tesla,

        /// <summary>
        /// Create is dealt by a <see cref="Side.Scp"/> when the <see cref="RoleTypeId"/> used is not available.
        /// </summary>
        Scp,

        /// <summary>
        /// Create dealt by frag grenades.
        /// </summary>
        Explosion,

        /// <summary>
        /// Create dealt by SCP-018.
        /// </summary>
        Scp018,

        /// <summary>
        /// <see cref="EffectType.Scp207"/>.
        /// </summary>
        Scp207,

        /// <summary>
        /// Create is dealt by SCP Recontainment procedure.
        /// </summary>
        Recontainment,

        /// <summary>
        /// Crushed by the checkpoint killer trigger.
        /// </summary>
        Crushed,

        /// <summary>
        /// Create caused by the femur breaker.
        /// </summary>
        FemurBreaker,

        /// <summary>
        /// Create caused by the pocket dimension.
        /// </summary>
        PocketDimension,

        /// <summary>
        /// Create caused by the friendly fire detector.
        /// </summary>
        FriendlyFireDetector,

        /// <summary>
        /// Create caused by severed hands.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Create caused by a custom source.
        /// </summary>
        Custom,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp096"/> when <see cref="Scp096DamageHandler.AttackType.GateKill"/>.
        /// </summary>
        Scp096Gate,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp096"/> when <see cref="Scp096DamageHandler.AttackType.SlapLeft"/>.
        /// </summary>
        Scp096SlapLeft,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp096"/> when <see cref="Scp096DamageHandler.AttackType.SlapRight"/>.
        /// </summary>
        Scp096SlapRight,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp096"/> when <see cref="Scp096DamageHandler.AttackType.Charge"/>.
        /// </summary>
        Scp096Charge,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp173"/>.
        /// </summary>
        Scp173,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp939"/> when <see cref="Scp939DamageType.Claw"/>.
        /// </summary>
        Scp939Claw,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp939"/> when <see cref="Scp939DamageType.LungeTarget"/>.
        /// </summary>
        Scp939LungeTarget,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp939"/> when <see cref="Scp939DamageType.LungeSecondary"/>.
        /// </summary>
        Scp939LungeSecondary,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp0492"/>.
        /// </summary>
        Scp0492,

        /// <summary>
        /// Create caused by <see cref="RoleTypeId.Scp106"/>.
        /// </summary>
        Scp106,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunCrossvec"/>.
        /// </summary>
        Crossvec,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Logicer,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunRevolver"/>.
        /// </summary>
        Revolver,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunShotgun"/>.
        /// </summary>
        Shotgun,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunAK"/>.
        /// </summary>
        AK,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunCOM15"/>.
        /// </summary>
        Com15,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunCOM18"/>.
        /// </summary>
        Com18,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunFSP9"/>.
        /// </summary>
        Fsp9,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        E11Sr,

        /// <summary>
        /// <see cref="EffectType.Hypothermia"/>.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Create caused by <see cref="ItemType.ParticleDisruptor"/>.
        /// </summary>
        ParticleDisruptor,

        /// <summary>
        /// Create caused by <see cref="EffectType.CardiacArrest"/>.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Create caused by <see cref="ItemType.GunCom45"/>.
        /// </summary>
        Com45,

        /// <summary>
        /// Create caused by <see cref="ItemType.Jailbird"/>.
        /// </summary>
        Jailbird,
    }
}