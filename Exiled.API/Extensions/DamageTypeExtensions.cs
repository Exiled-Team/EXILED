// -----------------------------------------------------------------------
// <copyright file="DamageTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using PlayerStatsSystem;

    /// <summary>
    /// A set of extensions for <see cref="DamageType"/>.
    /// </summary>
    public static class DamageTypeExtensions
    {
        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation"/>s and <see cref="DamageType"/>s.
        /// </summary>
        public static Dictionary<DeathTranslation, DamageType> TranslationConversion { get; } = new Dictionary<DeathTranslation, DamageType>
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
        /// <param name="checkItems">Indicates whether the SCP-items damage types should be taken into account or not.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by SCP or not.</returns>
        public static bool IsScp(this DamageType type, bool checkItems = true)
            => type == DamageType.Scp || type == DamageType.Scp049 ||
               type == DamageType.Scp096 || type == DamageType.Scp106 ||
               type == DamageType.Scp173 || type == DamageType.Scp939 ||
               type == DamageType.Scp0492 || (checkItems && (type == DamageType.Scp018 || type == DamageType.Scp207));

        /// <summary>
        /// Check if a <see cref="DamageType">damage type</see> is caused by status effect.
        /// </summary>
        /// <param name="type">The damage type to be checked.</param>
        /// <returns>Returns whether the <see cref="DamageType"/> is caused by status effect or not.</returns>
        public static bool IsStatusEffect(this DamageType type)
            => type == DamageType.Asphyxiation || type == DamageType.Poison ||
               type == DamageType.Bleeding || type == DamageType.Scp207 || type == DamageType.Hypothermia;

        /// <summary>
        /// Gets the <see cref="DamageType"/> of an <see cref="DamageHandlerBase"/>s.
        /// </summary>
        /// <param name="damageHandlerBase">The DamageHandler you want to get the DamageType.</param>
        /// <returns>Return the <see cref="DamageType"/> of the <see cref="DamageHandlerBase"/>.</returns>
        public static DamageType GetDamageType(DamageHandlerBase damageHandlerBase)
        {
            switch (damageHandlerBase)
            {
                case CustomReasonDamageHandler _:
                    return DamageType.Custom;
                case WarheadDamageHandler _:
                    return DamageType.Warhead;
                case ExplosionDamageHandler _:
                    return DamageType.Explosion;
                case Scp018DamageHandler _:
                    return DamageType.Scp018;
                case RecontainmentDamageHandler _:
                    return DamageType.Recontainment;
                case ScpDamageHandler scpDamageHandler:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[scpDamageHandler._translationId];
                        if (translation.Id == DeathTranslations.PocketDecay.Id)
                            return DamageType.Scp106;

                        if (TranslationConversion.ContainsKey(translation))
                            return TranslationConversion[translation];

                        return DamageType.Scp;
                    }

                case UniversalDamageHandler universal:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                        if (TranslationConversion.ContainsKey(translation))
                            return TranslationConversion[translation];
                        break;
                    }
            }

            return DamageType.Unknown;
        }
    }
}
