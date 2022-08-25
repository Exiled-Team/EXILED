// -----------------------------------------------------------------------
// <copyright file="DamageTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using PlayerStatsSystem;

    /// <summary>
    /// A set of extensions for <see cref="DamageType"/>.
    /// </summary>
    public static class DamageTypeExtensions
    {
        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation.Id"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static ReadOnlyDictionary<byte, DamageType> TranslationIdConversion { get; } = new(TranslationIdConversionInternal);

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static ReadOnlyDictionary<DeathTranslation, DamageType> TranslationConversion { get; } = new(TranslationConversionInternal);

        /// <summary>
        /// Gets conversion information between <see cref="ItemType"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static ReadOnlyDictionary<ItemType, DamageType> ItemConversion { get; } = new(ItemConversionInternal);

        private static Dictionary<byte, DamageType> TranslationIdConversionInternal { get; } = new()
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
            { DeathTranslations.Scp939.Id, DamageType.Scp939 },
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

        private static Dictionary<DeathTranslation, DamageType> TranslationConversionInternal { get; } = new()
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
            { DeathTranslations.Scp939, DamageType.Scp939 },
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

        private static Dictionary<ItemType, DamageType> ItemConversionInternal { get; } = new()
        {
            { ItemType.GunCrossvec, DamageType.Crossvec },
            { ItemType.GunLogicer, DamageType.Logicer },
            { ItemType.GunRevolver, DamageType.Revolver },
            { ItemType.GunShotgun, DamageType.Shotgun },
            { ItemType.GunAK, DamageType.AK },
            { ItemType.GunCOM15, DamageType.Com15 },
            { ItemType.GunCOM18, DamageType.Com18 },
            { ItemType.GunFSP9, DamageType.Fsp9 },
            { ItemType.GunE11SR, DamageType.E11Sr },
            { ItemType.MicroHID, DamageType.MicroHid },
            { ItemType.ParticleDisruptor, DamageType.ParticleDisruptor },
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by weapon.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkMicro">Indicates whether the MicroHid damage type should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by weapon or not.</returns>
        public static bool IsWeapon(this DamageType type, bool checkMicro = true) => type switch
        {
            DamageType.Crossvec or DamageType.Logicer or DamageType.Revolver or DamageType.Shotgun or DamageType.AK or DamageType.Com15 or DamageType.Com18 or DamageType.E11Sr or DamageType.Fsp9 or DamageType.ParticleDisruptor => true,
            DamageType.MicroHid when checkMicro => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by SCP.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <param name="checkItems">Indicates whether the SCP-items damage types should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP or not.</returns>
        public static bool IsScp(this DamageType type, bool checkItems = true) => type switch
        {
            DamageType.Scp or DamageType.Scp049 or DamageType.Scp096 or DamageType.Scp106 or DamageType.Scp173 or DamageType.Scp939 or DamageType.Scp0492 => true,
            DamageType.Scp018 or DamageType.Scp207 when checkItems => true,
            _ => false,
        };

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by status effect.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by status effect or not.</returns>
        public static bool IsStatusEffect(this DamageType type) => type switch
        {
            DamageType.Asphyxiation or DamageType.Poison or DamageType.Bleeding or DamageType.Scp207 or DamageType.Hypothermia => true,
            _ => false,
        };

        /// <summary>
        /// Gets the <see cref="DamageType"/> of an <see cref="DamageHandlerBase"/>s.
        /// </summary>
        /// <param name="damageHandlerBase">The DamageHandler you want to get the DamageType.</param>
        /// <returns>Return the <see cref="DamageType"/> of the <see cref="DamageHandlerBase"/>.</returns>
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

                case FirearmDamageHandler firearmDamageHandler:
                    {
                        return ItemConversion.ContainsKey(firearmDamageHandler.WeaponType) ? ItemConversion[firearmDamageHandler.WeaponType] : DamageType.Firearm;
                    }

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