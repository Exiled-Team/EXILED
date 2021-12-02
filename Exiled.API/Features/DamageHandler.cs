// -----------------------------------------------------------------------
// <copyright file="DamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Dissonance;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper for <see cref="DamageHandlerBase"/>.
    /// </summary>
    public class DamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="target">The <see cref="Player"/> target of the handler.</param>
        /// <param name="handlerBase">The <see cref="DamageHandlerBase"/> to initialize from.</param>
        public DamageHandler(Player target, DamageHandlerBase handlerBase)
        {
            Base = handlerBase;
            Target = target;
            Attacker = handlerBase is AttackerDamageHandler attacker ? Player.Get(attacker.Attacker.Hub) : null;
            Item = Attacker?.CurrentItem;
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
        public float Amount
        {
            get
            {
                if (Base is StandardDamageHandler standard)
                    return standard.Damage;
                else
                    return 0f;
            }

            set
            {
                if (Base is StandardDamageHandler standard)
                    standard.Damage = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DamageType"/> for the handler.
        /// </summary>
        public DamageType Type
        {
            get
            {
                if (Item != null)
                {
                    switch (Item)
                    {
                        case Firearm _:
                            return DamageType.Firearm;
                        case MicroHid _:
                            return DamageType.MicroHid;
                    }
                }
                else
                {
                    switch (Base)
                    {
                        case WarheadDamageHandler _:
                            return DamageType.Warhead;
                        case ScpDamageHandler scp:
                            return scp._translationId == DeathTranslations.PocketDecay.Id ? DamageType.PocketDimension : DamageType.Scp;
                        case ExplosionDamageHandler _:
                            return DamageType.Explosion;
                        case Scp018DamageHandler _:
                            return DamageType.Scp018;
                        case RecontainmentDamageHandler _:
                            return DamageType.Recontainment;
                        case UniversalDamageHandler universal:
                        {
                            DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];
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
