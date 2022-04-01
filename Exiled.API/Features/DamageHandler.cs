// -----------------------------------------------------------------------
// <copyright file="DamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System.Collections.Generic;

    using Dissonance;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper for <see cref="DamageHandlerBase"/>.
    /// </summary>
    public class DamageHandler {
        private readonly Dictionary<DeathTranslation, DamageType> translationConversion = new Dictionary<DeathTranslation, DamageType>
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
            };

        private readonly Dictionary<ItemType, DamageType> itemConversion = new Dictionary<ItemType, DamageType>
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
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="target">The <see cref="Player"/> target of the handler.</param>
        /// <param name="handlerBase">The <see cref="DamageHandlerBase"/> to initialize from.</param>
        public DamageHandler(Player target, DamageHandlerBase handlerBase) {
            Base = handlerBase;
            Target = target;
            Attacker = handlerBase is AttackerDamageHandler attacker ? Player.Get(attacker.Attacker.Hub) : null;
            Item = handlerBase is FirearmDamageHandler ? Attacker.CurrentItem : null;
        }

        /// <summary>
        /// Gets or sets the <see cref="DamageHandlerBase"/> base for this handler.
        /// </summary>
        public DamageHandlerBase Base { get; set; }

        /// <summary>
        /// Gets the <see cref="Player"/> target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/> attacker. CAN BE NULL!.
        /// </summary>
        public Player Attacker { get; set; }

        /// <summary>
        /// Gets or sets the amount of damage to be dealt.
        /// </summary>
        public float Amount {
            get {
                if (Base is StandardDamageHandler standard)
                    return standard.Damage;
                else
                    return 0f;
            }

            set {
                if (Base is StandardDamageHandler standard)
                    standard.Damage = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DamageType"/> for the handler.
        /// </summary>
        public DamageType Type {
            get {
                if (Item != null) {
                    switch (Item) {
                        case Firearm _:
                            if (Item != null && itemConversion.ContainsKey(Item.Type))
                                return itemConversion[Item.Type];

                            return DamageType.Firearm;
                        case MicroHid _:
                            return DamageType.MicroHid;
                    }
                }
                else {
                    switch (Base) {
                        case CustomReasonDamageHandler _:
                            return DamageType.Custom;
                        case WarheadDamageHandler _:
                            return DamageType.Warhead;
                        case Scp096DamageHandler _:
                            return DamageType.Scp096;
                        case ScpDamageHandler scp: {
                                DeathTranslation translation = DeathTranslations.TranslationsById[scp._translationId];
                                if (translation.Id == DeathTranslations.PocketDecay.Id)
                                    return DamageType.Scp106;
                                return translationConversion.ContainsKey(translation) ? translationConversion[translation] : DamageType.Scp;
                            }

                        case ExplosionDamageHandler _:
                            return DamageType.Explosion;
                        case Scp018DamageHandler _:
                            return DamageType.Scp018;
                        case RecontainmentDamageHandler _:
                            return DamageType.Recontainment;
                        case UniversalDamageHandler universal: {
                                DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                                if (translationConversion.ContainsKey(translation))
                                    return translationConversion[translation];
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

                                Log.Warn($"{nameof(DamageHandler)}.{nameof(Type)}: No matching {nameof(DamageType)} for {nameof(UniversalDamageHandler)} with ID {translation.Id}, type will be reported as {DamageType.Unknown}. Report this to EXILED Devs.");
                                break;
                            }
                    }
                }

                return DamageType.Unknown;
            }
        }

        /// <summary>
        /// Gets the <see cref="Item"/> used to create the damage handler. CAN BE NULL!.
        /// </summary>
        public Item Item { get; }

        /// <inheritdoc/>
        public override string ToString() => $"{Target} {Amount} ({Type}) {(Attacker != null ? Attacker.Nickname : "No one")} {(Item != null ? Item.ToString() : "No item")}";
    }
}
