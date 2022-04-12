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
        public static Type Type(this EffectType effect)
        {
            // Recursive patterns in C# 7.3, bruh
            switch (effect)
            {
                case EffectType.Amnesia: return typeof(Amnesia);
                case EffectType.Asphyxiated: return typeof(Asphyxiated);
                case EffectType.Bleeding: return typeof(Bleeding);
                case EffectType.Blinded: return typeof(Blinded);
                case EffectType.Burned: return typeof(Burned);
                case EffectType.Concussed: return typeof(Concussed);
                case EffectType.Corroding: return typeof(Corroding);
                case EffectType.Deafened: return typeof(Deafened);
                case EffectType.Decontaminating: return typeof(Decontaminating);
                case EffectType.Disabled: return typeof(Disabled);
                case EffectType.Ensnared: return typeof(Ensnared);
                case EffectType.Exhausted: return typeof(Exhausted);
                case EffectType.Flashed: return typeof(Flashed);
                case EffectType.Hemorrhage: return typeof(Hemorrhage);
                case EffectType.Invigorated: return typeof(Invigorated);
                case EffectType.BodyshotReduction: return typeof(BodyshotReduction);
                case EffectType.Poisoned: return typeof(Poisoned);
                case EffectType.Scp207: return typeof(Scp207);
                case EffectType.Invisible: return typeof(Invisible);
                case EffectType.SinkHole: return typeof(SinkHole);
                case EffectType.Visuals939: return typeof(Visuals939);
                case EffectType.DamageReduction: return typeof(DamageReduction);
                case EffectType.MovementBoost: return typeof(MovementBoost);
                case EffectType.RainbowTaste: return typeof(RainbowTaste);
                case EffectType.SeveredHands: return typeof(SeveredHands);
                case EffectType.Stained: return typeof(Stained);
                case EffectType.Visual173Blink: return typeof(Visuals173Blink);
                case EffectType.Vitality: return typeof(Vitality);
                case EffectType.Hypothermia: return typeof(Hypothermia);
                case EffectType.Scp1853: return typeof(Scp1853);
            }

            // This should never happen
            throw new InvalidOperationException("Invalid effect enum provided");
        }
    }
}
