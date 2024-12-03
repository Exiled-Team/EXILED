// -----------------------------------------------------------------------
// <copyright file="EffectTypeExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using CustomPlayerEffects;
    using CustomRendering;
    using Enums;
    using Features;
    using InventorySystem.Items.MarshmallowMan;
    using InventorySystem.Items.Usables.Scp244.Hypothermia;
    using PlayerRoles.FirstPersonControl;

    /// <summary>
    /// A set of extensions for <see cref="EffectType"/>.
    /// </summary>
    public static class EffectTypeExtension
    {
        /// <summary>
        /// Gets a dictionary that maps each <see cref="EffectType"/> to its corresponding <see cref="System.Type"/>.
        /// </summary>
        public static ReadOnlyDictionary<EffectType, Type> EffectTypeToType { get; } = new(new Dictionary<EffectType, Type>(45)
        {
            { EffectType.AmnesiaItems, typeof(AmnesiaItems) },
            { EffectType.AmnesiaVision, typeof(AmnesiaVision) },
            { EffectType.Asphyxiated, typeof(Asphyxiated) },
            { EffectType.Bleeding, typeof(Bleeding) },
            { EffectType.Blinded, typeof(Blindness) },
            { EffectType.BodyshotReduction, typeof(BodyshotReduction) },
            { EffectType.Burned, typeof(Burned) },
            { EffectType.CardiacArrest, typeof(CardiacArrest) },
            { EffectType.Concussed, typeof(Concussed) },
            { EffectType.PocketCorroding, typeof(PocketCorroding) },
            { EffectType.Corroding, typeof(Corroding) },
            { EffectType.DamageReduction, typeof(DamageReduction) },
            { EffectType.Deafened, typeof(Deafened) },
            { EffectType.Decontaminating, typeof(Decontaminating) },
            { EffectType.Disabled, typeof(Disabled) },
            { EffectType.Ensnared, typeof(Ensnared) },
            { EffectType.Exhausted, typeof(Exhausted) },
            { EffectType.Flashed, typeof(Flashed) },
            { EffectType.Hemorrhage, typeof(Hemorrhage) },
            { EffectType.Hypothermia, typeof(Hypothermia) },
            { EffectType.InsufficientLighting, typeof(InsufficientLighting) },
            { EffectType.Invigorated, typeof(Invigorated) },
            { EffectType.Invisible, typeof(Invisible) },
            { EffectType.MovementBoost, typeof(MovementBoost) },
            { EffectType.Poisoned, typeof(Poisoned) },
            { EffectType.RainbowTaste, typeof(RainbowTaste) },
            { EffectType.Scp207, typeof(Scp207) },
            { EffectType.Scp1853, typeof(Scp1853) },
            { EffectType.SeveredHands, typeof(SeveredHands) },
            { EffectType.SinkHole, typeof(Sinkhole) },
            { EffectType.Stained, typeof(Stained) },
            { EffectType.Vitality, typeof(Vitality) },
            { EffectType.SoundtrackMute, typeof(SoundtrackMute) },
            { EffectType.SpawnProtected, typeof(SpawnProtected) },
            { EffectType.Traumatized, typeof(Traumatized) },
            { EffectType.AntiScp207, typeof(AntiScp207) },
            { EffectType.Scanned, typeof(Scanned) },
            { EffectType.SilentWalk, typeof(SilentWalk) },
#pragma warning disable CS0618
            { EffectType.Marshmallow, typeof(MarshmallowEffect) },
#pragma warning restore CS0618
            { EffectType.Strangled, typeof(Strangled) },
            { EffectType.Ghostly, typeof(Ghostly) },
            { EffectType.FogControl, typeof(FogControl) },
            { EffectType.Slowness, typeof(Slowness) },
            { EffectType.Scp1344, typeof(Scp1344) },
            { EffectType.SeveredEyes, typeof(SeveredEyes) },
            { EffectType.PitDeath, typeof(PitDeath) },
            { EffectType.Blurred, typeof(Blurred) },
        });

        /// <summary>
        /// Gets a dictionary that maps each <see cref="System.Type"/> to its corresponding <see cref="EffectType"/>.
        /// </summary>
        public static ReadOnlyDictionary<Type, EffectType> TypeToEffectType { get; } = new(EffectTypeToType.ToDictionary(x => x.Value, y => y.Key));

        /// <summary>
        /// Gets an instance of <see cref="System.Type"/> points to an effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> enum.</param>
        /// <returns>The <see cref="System.Type"/>.</returns>
        public static Type Type(this EffectType effect)
            => EffectTypeToType.TryGetValue(effect, out Type type) ? type : throw new InvalidOperationException("Invalid effect enum provided");

        /// <summary>
        /// Gets an instance of <see cref="System.Type"/> points to an effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> enum.</param>
        /// <param name="type">The type found with the corresponding EffecType.</param>
        /// <returns>Whether or not the effectType has been found.</returns>
        public static bool TryGetType(this EffectType effect, out Type type)
            => EffectTypeToType.TryGetValue(effect, out type);

        /// <summary>
        /// Gets the <see cref="EffectType"/> of the specified <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="statusEffectBase">The <see cref="StatusEffectBase"/> enum.</param>
        /// <returns>The <see cref="EffectType"/>.</returns>
        public static EffectType GetEffectType(this StatusEffectBase statusEffectBase)
            => TypeToEffectType.TryGetValue(statusEffectBase.GetType(), out EffectType effect) ? effect : throw new InvalidOperationException("Invalid effect status base provided");

        /// <summary>
        /// Gets the <see cref="EffectType"/> of the specified <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="statusEffectBase">The <see cref="StatusEffectBase"/> enum.</param>
        /// <param name="effect">The effect found.</param>
        /// <returns>Whether or not the effect has been found.</returns>
        public static bool TryGetEffectType(this StatusEffectBase statusEffectBase, out EffectType effect)
        {
            if (statusEffectBase == null || !TypeToEffectType.TryGetValue(statusEffectBase.GetType(), out effect))
            {
                effect = EffectType.None;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="FogType"/> of the specified <see cref="FogControl"/>.
        /// </summary>
        /// <param name="fogControl">The <see cref="FogControl"/> effect.</param>
        /// <param name="fogType">The <see cref="FogType"/> applied.</param>
        public static void SetFogType(this FogControl fogControl, FogType fogType) => fogControl.Intensity = (byte)(fogType + 1);

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> drains health over time.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect drains health over time.</returns>
        /// <seealso cref="IsHealing(EffectType)"/>
        public static bool IsHarmful(this EffectType effect) => effect is EffectType.Asphyxiated or EffectType.Bleeding
            or EffectType.Corroding or EffectType.Decontaminating or EffectType.Hemorrhage or EffectType.Hypothermia
            or EffectType.Poisoned or EffectType.Scp207 or EffectType.SeveredHands or EffectType.Strangled;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> heals a player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect heals.</returns>
        /// <seealso cref="IsHarmful(EffectType)"/>
        public static bool IsHealing(this EffectType effect) => effect.TryGetType(out Type type) && typeof(IHealableEffect).IsAssignableFrom(type);

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a negative effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a negative effect.</returns>
        /// <seealso cref="IsHarmful(EffectType)"/>
        public static bool IsNegative(this EffectType effect) => IsHarmful(effect) || GetEffectBase(effect)?.Classification == StatusEffectBase.EffectClassification.Negative;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a positive effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a positive effect.</returns>
        /// <seealso cref="IsHealing(EffectType)"/>
        public static bool IsPositive(this EffectType effect) => GetEffectBase(effect)?.Classification == StatusEffectBase.EffectClassification.Positive;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> affects the player's movement speed.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect modifies the player's movement speed.</returns>
        public static bool IsMovement(this EffectType effect) => effect.TryGetType(out Type type) && typeof(IMovementSpeedModifier).IsAssignableFrom(type);

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is displayed to spectators as text.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is displayed to spectators as text.</returns>
        public static bool IsDisplayed(this EffectType effect) => effect.TryGetType(out Type type) && typeof(ISpectatorDataPlayerEffect).IsAssignableFrom(type);

        /// <summary>
        /// Returns the <see cref="StatusEffectBase"/> of the given <paramref name="effect"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="StatusEffectBase"/> of the <see cref="EffectType"/>.</returns>
        public static StatusEffectBase GetEffectBase(this EffectType effect) => TryGetEffectBase(effect, out StatusEffectBase effectBase) ? effectBase : null;

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> of the given <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <param name="effectBase">The <see cref="StatusEffectBase"/> to return.</param>
        /// <returns><see langword="true"/> if a <see cref="StatusEffectBase"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEffectBase(this EffectType effect, out StatusEffectBase effectBase) => Server.Host.TryGetEffect(effect, out effectBase);

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> of the given type <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="effectBase">The <see cref="StatusEffectBase"/>.</param>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to get.</typeparam>
        /// <returns><see langword="true"/> if a <see cref="StatusEffectBase"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEffectBase<T>(out T effectBase)
            where T : StatusEffectBase => Server.Host.TryGetEffect(out effectBase);

        /// <summary>
        /// Returns the <see cref="EffectCategory"/> of the given <paramref name="effect"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="EffectCategory"/> representing the effect.</returns>
        public static EffectCategory GetCategories(this EffectType effect)
        {
            EffectCategory category = EffectCategory.None;
            if (effect.IsPositive())
                category = category.AddFlags(EffectCategory.Positive);
            if (effect.IsNegative())
                category = category.AddFlags(EffectCategory.Negative);
            if (effect.IsMovement())
                category = category.AddFlags(EffectCategory.Movement);
            if (effect.IsHarmful())
                category = category.AddFlags(EffectCategory.Harmful);

            return category;
        }
    }
}