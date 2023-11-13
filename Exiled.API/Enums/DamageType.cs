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
        /// Damage dealt by a <see cref="Features.Items.Firearm"/> when the <see cref="ItemType"/> used is not available.
        /// </summary>
        Firearm,

        /// <summary>
        /// Damage dealt by a <see cref="Features.Items.MicroHid"/>.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Damage dealt by a Tesla Gate.
        /// </summary>
        Tesla,

        /// <summary>
        /// Damage is dealt by a <see cref="Side.Scp"/> when the <see cref="RoleTypeId"/> used is not available.
        /// </summary>
        Scp,

        /// <summary>
        /// Damage dealt by frag grenades.
        /// </summary>
        Explosion,

        /// <summary>
        /// Damage dealt by SCP-018.
        /// </summary>
        Scp018,

        /// <summary>
        /// <see cref="EffectType.Scp207"/>.
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
        /// Damage caused by a custom source.
        /// </summary>
        Custom,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp096"/>.
        /// </summary>
        Scp096,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp173"/>.
        /// </summary>
        Scp173,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp939"/>.
        /// </summary>
        Scp939,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp0492"/>.
        /// </summary>
        Scp0492,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp106"/>.
        /// </summary>
        Scp106,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunCrossvec"/>.
        /// </summary>
        Crossvec,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        Logicer,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunRevolver"/>.
        /// </summary>
        Revolver,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunShotgun"/>.
        /// </summary>
        Shotgun,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunAK"/>.
        /// </summary>
        AK,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunCOM15"/>.
        /// </summary>
        Com15,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunCOM18"/>.
        /// </summary>
        Com18,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunFSP9"/>.
        /// </summary>
        Fsp9,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        E11Sr,

        /// <summary>
        /// <see cref="EffectType.Hypothermia"/>.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Damage caused by <see cref="ItemType.ParticleDisruptor"/>.
        /// </summary>
        ParticleDisruptor,

        /// <summary>
        /// Damage caused by <see cref="EffectType.CardiacArrest"/>.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunCom45"/>.
        /// </summary>
        Com45,

        /// <summary>
        /// Damage caused by <see cref="ItemType.Jailbird"/>.
        /// </summary>
        Jailbird,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunFRMG0"/>.
        /// </summary>
        Frmg0,

        /// <summary>
        /// Damage caused by <see cref="ItemType.GunA7"/>.
        /// </summary>
        A7,

        /// <summary>
        /// Damage caused by <see cref="RoleTypeId.Scp3114"/>
        /// </summary>
        Scp3114,

        /// <summary>
        /// Damage caused by Scp3114's strangling ability.
        /// </summary>
        Strangled,

        /// <summary>
        /// Damage caused by the marshmallow man.
        /// </summary>
        Marshmallow,
    }
}
