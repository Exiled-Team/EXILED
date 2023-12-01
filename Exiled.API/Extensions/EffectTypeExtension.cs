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
    using System.Linq;

    using CustomPlayerEffects;
    using Enums;
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
        public static Dictionary<EffectType, Type> EffectTypeToType { get; } = new(35)
        {
            { EffectType.AmnesiaItems, typeof(AmnesiaItems) },
            { EffectType.AmnesiaVision, typeof(AmnesiaVision) },
            { EffectType.Asphyxiated, typeof(Asphyxiated) },
            { EffectType.Bleeding, typeof(Bleeding) },
            { EffectType.Blinded, typeof(Blinded) },
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
        };

        /// <summary>
        /// Gets a dictionary that maps each <see cref="System.Type"/> to its corresponding <see cref="EffectType"/>.
        /// </summary>
        public static Dictionary<Type, EffectType> TypeToEffectType { get; } = EffectTypeToType.ToDictionary(x => x.Value, y => y.Key);

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
        public static bool IsHealing(this EffectType effect) => effect.TryGetType(out Type type) && typeof(IHealablePlayerEffect).IsAssignableFrom(type);

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a negative effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a negative effect.</returns>
        /// <seealso cref="IsHarmful(EffectType)"/>
        public static bool IsNegative(this EffectType effect) => IsHarmful(effect) || effect is EffectType.AmnesiaItems
            or EffectType.AmnesiaVision or EffectType.Blinded or EffectType.Burned or EffectType.Concussed or EffectType.Deafened
            or EffectType.Disabled or EffectType.Ensnared or EffectType.Exhausted or EffectType.Flashed or EffectType.SinkHole
            or EffectType.Stained or EffectType.InsufficientLighting or EffectType.SoundtrackMute or EffectType.Scanned;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a positive effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a positive effect.</returns>
        /// <seealso cref="IsHealing(EffectType)"/>
        public static bool IsPositive(this EffectType effect) => effect is EffectType.BodyshotReduction or EffectType.DamageReduction
            or EffectType.Invigorated or EffectType.Invisible or EffectType.MovementBoost or EffectType.RainbowTaste
            or EffectType.Scp207 or EffectType.Scp1853 or EffectType.Vitality or EffectType.AntiScp207 or EffectType.Ghostly;

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
        /// Returns the <see cref="EffectCategory"/> of the given <paramref name="effect"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="EffectCategory"/> representing the effect.</returns>
        public static EffectCategory GetCategories(this EffectType effect)
        {
            EffectCategory category = EffectCategory.None;
            if (effect.IsPositive())
                category |= EffectCategory.Positive;
            if (effect.IsNegative())
                category |= EffectCategory.Negative;
            if (effect.IsMovement())
                category |= EffectCategory.Movement;
            if (effect.IsHarmful())
                category |= EffectCategory.Harmful;

            return category;
        }
    }
}
