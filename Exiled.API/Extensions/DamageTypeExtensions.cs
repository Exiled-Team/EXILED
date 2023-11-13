// -----------------------------------------------------------------------
// <copyright file="DamageTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    using Enums;

    using Features;

    using PlayerStatsSystem;

    /// <summary>
    /// A set of extensions for <see cref="DamageType"/>.
    /// </summary>
    public static class DamageTypeExtensions
    {
        private static readonly Dictionary<byte, DamageType> TranslationIdConversionInternal = new()
        {
            { DeathTranslations.Asphyxiated.Id, DamageType.Asphyxiation },
            { DeathTranslations.Bleeding.Id, DamageType.Bleeding },
            { DeathTranslations.Crushed.Id, DamageType.Crushed },
            { DeathTranslations.Decontamination.Id, DamageType.Decontamination },
            { DeathTranslations.Explosion.Id, DamageType.Explosion },
            { DeathTranslations.Falldown.Id, DamageType.Falldown },
            { DeathTranslations.Poisoned.Id, DamageType.Poison },
            { DeathTranslations.Recontained.Id, DamageType.Recontainment },
            { DeathTranslations.Scp049.Id, DamageType.Scp049 },
            { DeathTranslations.Scp096.Id, DamageType.Scp096 },
            { DeathTranslations.Scp173.Id, DamageType.Scp173 },
            { DeathTranslations.Scp207.Id, DamageType.Scp207 },
            { DeathTranslations.Scp939Lunge.Id, DamageType.Scp939 },
            { DeathTranslations.Scp939Other.Id, DamageType.Scp939 },
            { DeathTranslations.Tesla.Id, DamageType.Tesla },
            { DeathTranslations.Unknown.Id, DamageType.Unknown },
            { DeathTranslations.Warhead.Id, DamageType.Warhead },
            { DeathTranslations.Zombie.Id, DamageType.Scp0492 },
            { DeathTranslations.BulletWounds.Id, DamageType.Firearm },
            { DeathTranslations.PocketDecay.Id, DamageType.PocketDimension },
            { DeathTranslations.SeveredHands.Id, DamageType.SeveredHands },
            { DeathTranslations.FriendlyFireDetector.Id, DamageType.FriendlyFireDetector },
            { DeathTranslations.UsedAs106Bait.Id, DamageType.FemurBreaker },
            { DeathTranslations.MicroHID.Id, DamageType.MicroHid },
            { DeathTranslations.Hypothermia.Id, DamageType.Hypothermia },
        };

        private static readonly Dictionary<DeathTranslation, DamageType> TranslationConversionInternal = new()
        {
            { DeathTranslations.Asphyxiated, DamageType.Asphyxiation },
            { DeathTranslations.Bleeding, DamageType.Bleeding },
            { DeathTranslations.Crushed, DamageType.Crushed },
            { DeathTranslations.Decontamination, DamageType.Decontamination },
            { DeathTranslations.Explosion, DamageType.Explosion },
            { DeathTranslations.Falldown, DamageType.Falldown },
            { DeathTranslations.Poisoned, DamageType.Poison },
            { DeathTranslations.Recontained, DamageType.Recontainment },
            { DeathTranslations.Scp049, DamageType.Scp049 },
            { DeathTranslations.Scp096, DamageType.Scp096 },
            { DeathTranslations.Scp173, DamageType.Scp173 },
            { DeathTranslations.Scp207, DamageType.Scp207 },
            { DeathTranslations.Scp939Lunge, DamageType.Scp939 },
            { DeathTranslations.Scp939Other, DamageType.Scp939 },
            { DeathTranslations.Tesla, DamageType.Tesla },
            { DeathTranslations.Unknown, DamageType.Unknown },
            { DeathTranslations.Warhead, DamageType.Warhead },
            { DeathTranslations.Zombie, DamageType.Scp0492 },
            { DeathTranslations.BulletWounds, DamageType.Firearm },
            { DeathTranslations.PocketDecay, DamageType.PocketDimension },
            { DeathTranslations.SeveredHands, DamageType.SeveredHands },
            { DeathTranslations.FriendlyFireDetector, DamageType.FriendlyFireDetector },
            { DeathTranslations.UsedAs106Bait, DamageType.FemurBreaker },
            { DeathTranslations.MicroHID, DamageType.MicroHid },
            { DeathTranslations.Hypothermia, DamageType.Hypothermia },
        };

        private static readonly Dictionary<ItemType, DamageType> ItemConversionInternal = new()
        {
            { ItemType.GunCrossvec, DamageType.Crossvec },
            { ItemType.GunLogicer, DamageType.Logicer },
            { ItemType.GunRevolver, DamageType.Revolver },
            { ItemType.GunShotgun, DamageType.Shotgun },
            { ItemType.GunAK, DamageType.AK },
            { ItemType.GunCOM15, DamageType.Com15 },
            { ItemType.GunCom45, DamageType.Com45 },
            { ItemType.GunCOM18, DamageType.Com18 },
            { ItemType.GunFSP9, DamageType.Fsp9 },
            { ItemType.GunE11SR, DamageType.E11Sr },
            { ItemType.MicroHID, DamageType.MicroHid },
            { ItemType.ParticleDisruptor, DamageType.ParticleDisruptor },
            { ItemType.Jailbird, DamageType.Jailbird },
            { ItemType.GunFRMG0, DamageType.Frmg0 },
            { ItemType.GunA7, DamageType.A7 },
        };

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation.Id"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<byte, DamageType> TranslationIdConversion => TranslationIdConversionInternal;

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<DeathTranslation, DamageType> TranslationConversion => TranslationConversionInternal;

        /// <summary>
        /// Gets conversion information between <see cref="ItemType"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static IReadOnlyDictionary<ItemType, DamageType> ItemConversion => ItemConversionInternal;

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a weapon.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkMicro">Indicates whether or not the MicroHid damage type should be taken into account.</param>
        /// <returns>Returns whether or not the <see cref="DamageType"/> is caused by weapon.</returns>
        public static bool IsWeapon(this DamageType type, bool checkMicro = true) => type switch
        {
            DamageType.Crossvec or DamageType.Logicer or DamageType.Revolver or DamageType.Shotgun or DamageType.AK or DamageType.Com15 or DamageType.Com18 or DamageType.E11Sr or DamageType.Fsp9 or DamageType.ParticleDisruptor or DamageType.Com45 or DamageType.Frmg0 or DamageType.A7 => true,
            DamageType.MicroHid when checkMicro => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a SCP.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkItems">Indicates whether or not the SCP-items damage types should be taken into account.</param>
        /// <returns>Returns whether or not the <see cref="DamageType"/> is caused by SCP.</returns>
        public static bool IsScp(this DamageType type, bool checkItems = true) => type switch
        {
            DamageType.Scp or DamageType.Scp049 or DamageType.Scp096 or DamageType.Scp106 or DamageType.Scp173 or DamageType.Scp939 or DamageType.Scp0492 => true,
            DamageType.Scp018 or DamageType.Scp207 when checkItems => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by a status effect.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether or not the <see cref="DamageType"/> is caused by status effect.</returns>
        public static bool IsStatusEffect(this DamageType type) => type switch
        {
            DamageType.Asphyxiation or DamageType.Poison or DamageType.Bleeding or DamageType.Scp207 or DamageType.Hypothermia => true,
            _ => false,
        };

        /// <summary>
        /// Gets the <see cref="DamageType"/> of an <see cref="DamageHandlerBase"/>s.
        /// </summary>
        /// <param name="damageHandlerBase">The DamageHandler to convert.</param>
        /// <returns>The <see cref="DamageType"/> of the <see cref="DamageHandlerBase"/>.</returns>
        public static DamageType GetDamageType(DamageHandlerBase damageHandlerBase)
        {
            switch (damageHandlerBase)
            {
                case CustomReasonDamageHandler:
                    return DamageType.Custom;
                case WarheadDamageHandler:
                    return DamageType.Warhead;
                case ExplosionDamageHandler:
                    return DamageType.Explosion;
                case Scp018DamageHandler:
                    return DamageType.Scp018;
                case RecontainmentDamageHandler:
                    return DamageType.Recontainment;
                case Scp096DamageHandler:
                    return DamageType.Scp096;
                case MicroHidDamageHandler:
                    return DamageType.MicroHid;
                case DisruptorDamageHandler:
                    return DamageType.ParticleDisruptor;
                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                        Scp049DamageHandler.AttackType.Instakill => DamageType.Scp049,
                        Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                        _ => DamageType.Unknown,
                    };
                case FirearmDamageHandler firearmDamageHandler:
                    return ItemConversion.ContainsKey(firearmDamageHandler.WeaponType) ? ItemConversion[firearmDamageHandler.WeaponType] : DamageType.Firearm;

                case ScpDamageHandler scpDamageHandler:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[scpDamageHandler._translationId];
                        if (translation.Id == DeathTranslations.PocketDecay.Id)
                            return DamageType.Scp106;

                        return TranslationIdConversion.ContainsKey(translation.Id)
                            ? TranslationIdConversion[translation.Id]
                            : DamageType.Scp;
                    }

                case UniversalDamageHandler universal:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                        if (TranslationIdConversion.ContainsKey(translation.Id))
                            return TranslationIdConversion[translation.Id];

                        Log.Warn($"{nameof(DamageTypeExtensions)}.{nameof(damageHandlerBase)}: No matching {nameof(DamageType)} for {nameof(UniversalDamageHandler)} with ID {translation.Id}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");
                        return DamageType.Unknown;
                    }
            }

            return DamageType.Unknown;
        }
    }
}
