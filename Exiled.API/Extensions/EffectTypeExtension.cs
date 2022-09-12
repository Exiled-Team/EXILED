// -----------------------------------------------------------------------
// <copyright file="EffectTypeExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using CustomPlayerEffects;

    using Exiled.API.Enums;

    using InventorySystem.Items.Usables.Scp244.Hypothermia;

    /// <summary>
    /// Contains an extension method to get <see cref="System.Type"/> from <see cref="EffectType"/>.
    /// </summary>
    public static class EffectTypeExtension
    {
        /// <summary>
        /// Gets an instance of <see cref="System.Type"/> points to an effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> enum.</param>
        /// <returns>The <see cref="System.Type"/>.</returns>
        public static Type Type(this EffectType effect) => effect switch
        {
            EffectType.Amnesia => typeof(Amnesia),
            EffectType.Asphyxiated => typeof(Asphyxiated),
            EffectType.Bleeding => typeof(Bleeding),
            EffectType.Blinded => typeof(Blinded),
            EffectType.Burned => typeof(Burned),
            EffectType.Concussed => typeof(Concussed),
            EffectType.Corroding => typeof(Corroding),
            EffectType.Deafened => typeof(Deafened),
            EffectType.Decontaminating => typeof(Decontaminating),
            EffectType.Disabled => typeof(Disabled),
            EffectType.Ensnared => typeof(Ensnared),
            EffectType.Exhausted => typeof(Exhausted),
            EffectType.Flashed => typeof(Flashed),
            EffectType.Hemorrhage => typeof(Hemorrhage),
            EffectType.Invigorated => typeof(Invigorated),
            EffectType.BodyshotReduction => typeof(BodyshotReduction),
            EffectType.Poisoned => typeof(Poisoned),
            EffectType.Scp207 => typeof(Scp207),
            EffectType.Invisible => typeof(Invisible),
            EffectType.SinkHole => typeof(SinkHole),
            EffectType.Visuals939 => typeof(Visuals939),
            EffectType.DamageReduction => typeof(DamageReduction),
            EffectType.MovementBoost => typeof(MovementBoost),
            EffectType.RainbowTaste => typeof(RainbowTaste),
            EffectType.SeveredHands => typeof(SeveredHands),
            EffectType.Stained => typeof(Stained),
            EffectType.Visuals173Blink => typeof(Visuals173Blink),
            EffectType.Vitality => typeof(Vitality),
            EffectType.Hypothermia => typeof(Hypothermia),
            EffectType.Scp1853 => typeof(Scp1853),

            // This should never happen
            _ => throw new InvalidOperationException("Invalid effect enum provided"),
        };

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> drains health over time.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect drains health over time.</returns>
        /// <seealso cref="IsHealing(EffectType)"/>
        public static bool IsHarmful(this EffectType effect) => typeof(IDamageModifierEffect).IsAssignableFrom(effect.Type());

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> heals a player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect heals.</returns>
        /// <seealso cref="IsHarmful(EffectType)"/>
        public static bool IsHealing(this EffectType effect) => typeof(IHealablePlayerEffect).IsAssignableFrom(effect.Type());

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a negative effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a negative effect.</returns>
        /// <seealso cref="IsHarmful(EffectType)"/>
        public static bool IsNegative(this EffectType effect) => IsHarmful(effect) || effect is EffectType.Amnesia
            or EffectType.Blinded or EffectType.Burned or EffectType.Concussed or EffectType.Deafened
            or EffectType.Disabled or EffectType.Ensnared or EffectType.Exhausted or EffectType.Flashed or EffectType.SinkHole
            or EffectType.Stained or EffectType.Visuals173Blink;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is a positive effect.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is a positive effect.</returns>
        /// <seealso cref="IsHealing(EffectType)"/>
        public static bool IsPositive(this EffectType effect) => effect is EffectType.BodyshotReduction or EffectType.DamageReduction
            or EffectType.Invigorated or EffectType.Invisible or EffectType.MovementBoost or EffectType.RainbowTaste
            or EffectType.Scp207 or EffectType.Scp1853 or EffectType.Vitality;

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> affects the player's movement speed.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect modifies the player's movement speed.</returns>
        public static bool IsMovement(this EffectType effect) => typeof(IMovementSpeedEffect).IsAssignableFrom(effect.Type());

        /// <summary>
        /// Returns whether or not the provided <paramref name="effect"/> is displayed to spectators as text.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>Whether or not the effect is displayed to spectators as text.</returns>
        public static bool IsDisplayed(this EffectType effect) => typeof(IDisplayablePlayerEffect).IsAssignableFrom(effect.Type());

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