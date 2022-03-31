// -----------------------------------------------------------------------
// <copyright file="DamageHandlerBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Exiled.API.Enums;

    using PlayerStatsSystem;

    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    public abstract class DamageHandlerBase
    {
        private DamageType damageType;
        private CassieAnnouncement cassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandlerBase"/> class.
        /// </summary>
        protected DamageHandlerBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandlerBase"/> class.
        /// </summary>
        /// <param name="baseHandler">The base <see cref="BaseHandler"/>.</param>
        protected DamageHandlerBase(BaseHandler baseHandler) => Base = baseHandler;

        /// <summary>
        /// All available <see cref="DamageHandler"/> actions.
        /// </summary>
        public enum Action : byte
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// The result is determined by a damage action.
            /// </summary>
            Damage,

            /// <summary>
            /// The result is determined by a death action.
            /// </summary>
            Death,
        }

        /// <summary>
        /// Gets or sets the base <see cref="BaseHandler"/>.
        /// </summary>
        public BaseHandler Base { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="CassieAnnouncement"/> belonging to this <see cref="DamageHandler"/> instance.
        /// </summary>
        public virtual CassieAnnouncement CassieDeathAnnouncement
        {
            get => cassieAnnouncement ?? Base.CassieDeathAnnouncement;
            protected set => cassieAnnouncement = value;
        }

        /// <summary>
        /// Gets the text to show in the server logs.
        /// </summary>
        public virtual string ServerLogsText => Base.ServerLogsText;

        /// <summary>
        /// Gets or sets the <see cref="DamageType"/> for the damage handler.
        /// </summary>
        public virtual DamageType Type
        {
            get
            {
                if (damageType != DamageType.Unknown)
                    return damageType;

                switch (Base)
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
                    case UniversalDamageHandler universal:
                        {
                            DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                            if (TranslationConversion.ContainsKey(translation))
                                return TranslationConversion[translation];

                            if (translation.Id == DeathTranslations.Asphyxiated.Id)
                                return DamageType.Asphyxiation;
                            if (translation.Id == DeathTranslations.Bleeding.Id)
                                return DamageType.Bleeding;
                            if (translation.Id == DeathTranslations.Decontamination.Id)
                                return DamageType.Decontamination;
                            if (translation.Id == DeathTranslations.Poisoned.Id)
                                return DamageType.Poison;
                            if (translation.Id == DeathTranslations.Falldown.Id)
                                return DamageType.Falldown;
                            if (translation.Id == DeathTranslations.Tesla.Id)
                                return DamageType.Tesla;
                            if (translation.Id == DeathTranslations.Scp207.Id)
                                return DamageType.Scp207;
                            if (translation.Id == DeathTranslations.Crushed.Id)
                                return DamageType.Crushed;
                            if (translation.Id == DeathTranslations.UsedAs106Bait.Id)
                                return DamageType.FemurBreaker;
                            if (translation.Id == DeathTranslations.FriendlyFireDetector.Id)
                                return DamageType.FriendlyFireDetector;
                            if (translation.Id == DeathTranslations.SeveredHands.Id)
                                return DamageType.SeveredHands;
                            if (translation.Id == DeathTranslations.Hypothermia.Id)
                                return DamageType.Hypothermia;

                            Log.Warn($"{nameof(DamageHandler)}.{nameof(Type)}: No matching {nameof(DamageType)} for {nameof(UniversalDamageHandler)} with ID {translation.Id}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");
                            break;
                        }
                }

                return DamageType.Unknown;
            }

            protected set
            {
                if (!Enum.IsDefined(typeof(DamageType), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DamageType));

                damageType = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PlayerStatsSystem.DeathTranslation"/>.
        /// </summary>
        public virtual DeathTranslation DeathTranslation => TranslationConversion.FirstOrDefault(translation => translation.Value == Type).Key;

        /// <summary>
        /// Gets conversion information between <see cref="DeathTranslation"/>s and <see cref="DamageType"/>s.
        /// </summary>
        internal static Dictionary<DeathTranslation, DamageType> TranslationConversion { get; } = new()
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
        /// Implicitly converts the given <see cref="DamageHandlerBase"/> instance to a <see cref="BaseHandler"/> object.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="DamageHandlerBase"/> instance.</param>
        public static implicit operator BaseHandler(DamageHandlerBase damageHandlerBase) => damageHandlerBase.Base;

        /// <summary>
        /// Applies the damage to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to damage.</param>
        /// <returns>The <see cref="Action"/> of the call to this method.</returns>
        public abstract Action ApplyDamage(Player player);

        /// <summary>
        /// Computes and processes the damage.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to damage.</param>
        public virtual void ProcessDamage(Player player)
        {
        }

        /// <summary>
        /// Unsafely casts the damage handler to the specified <see cref="BaseHandler"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="BaseHandler"/> type.</typeparam>
        /// <returns>A <see cref="BaseHandler"/> object.</returns>
        public T As<T>()
            where T : BaseHandler => Base as T;

        /// <summary>
        /// Unsafely casts the damage handler to the specified <see cref="DamageHandlerBase"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="DamageHandlerBase"/> type.</typeparam>
        /// <returns>A <see cref="DamageHandlerBase"/> object.</returns>
        public T BaseAs<T>()
            where T : DamageHandlerBase => this as T;

        /// <summary>
        /// Safely casts the damage handler to the specified <see cref="BaseHandler"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="BaseHandler"/> type.</typeparam>
        /// <param name="param">The casted <see cref="BaseHandler"/>.</param>
        /// <returns>A <see cref="BaseHandler"/> object.</returns>
        public bool Is<T>(out T param)
            where T : BaseHandler
        {
            param = default;

            if (!(Base is T cast))
                return false;

            param = cast;
            return true;
        }

        /// <summary>
        /// Safely casts the damage handler to the specified <see cref="DamageHandlerBase"/> type.
        /// </summary>
        /// <typeparam name="T">The specified <see cref="DamageHandlerBase"/> type.</typeparam>
        /// <param name="param">The casted <see cref="DamageHandlerBase"/>.</param>
        /// <returns>A <see cref="DamageHandlerBase"/> object.</returns>
        public bool BaseIs<T>(out T param)
            where T : DamageHandlerBase
        {
            param = default;

            if (!(this is T cast))
                return false;

            param = cast;
            return true;
        }

        /// <summary>
        /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler.CassieAnnouncement"/>.
        /// </summary>
        public class CassieAnnouncement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CassieAnnouncement"/> class.
            /// </summary>
            /// <param name="announcement">The announcement to be set.</param>
            public CassieAnnouncement(string announcement) => Announcement = announcement;

            /// <summary>
            /// Initializes a new instance of the <see cref="CassieAnnouncement"/> class.
            /// </summary>
            /// <param name="announcement">The announcement to be set.</param>
            /// <param name="subtitleParts">The subtitles to be set.</param>
            public CassieAnnouncement(string announcement, IEnumerable<Subtitles.SubtitlePart> subtitleParts)
                : this(announcement) => SubtitleParts = subtitleParts;

            /// <summary>
            /// Gets the default announcement.
            /// </summary>
            public static CassieAnnouncement Default => BaseHandler.CassieAnnouncement.Default;

            /// <summary>
            /// Gets or sets the announcement.
            /// </summary>
            public string Announcement { get; set; }

            /// <summary>
            /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="Subtitles.SubtitlePart"/> which determines the result of the subtitle belonging to this <see cref="CassieAnnouncement"/> instance.
            /// </summary>
            public IEnumerable<Subtitles.SubtitlePart> SubtitleParts { get; set; }

            /// <summary>
            /// Implicitly converts the given <see cref="CassieAnnouncement"/> instance to a <see cref="BaseHandler.CassieAnnouncement"/> object.
            /// </summary>
            /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> instance.</param>
            public static implicit operator BaseHandler.CassieAnnouncement(CassieAnnouncement cassieAnnouncement) =>
                new()
                {
                    Announcement = cassieAnnouncement.Announcement,
                    SubtitleParts = cassieAnnouncement.SubtitleParts.ToArray(),
                };

            /// <summary>
            /// Implicitly converts the given <see cref="BaseHandler.CassieAnnouncement"/> instance to a <see cref="CassieAnnouncement"/> object.
            /// </summary>
            /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> instance.</param>
            public static implicit operator CassieAnnouncement(BaseHandler.CassieAnnouncement cassieAnnouncement) =>
                new(cassieAnnouncement.Announcement, cassieAnnouncement.SubtitleParts);
        }
    }
}
