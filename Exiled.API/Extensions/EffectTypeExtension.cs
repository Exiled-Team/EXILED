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
                EffectType.Visual173Blink => typeof(Visuals173Blink),
                EffectType.Vitality => typeof(Vitality),
                EffectType.Hypothermia => typeof(Hypothermia),
                EffectType.Scp1853 => typeof(Scp1853),

                // This should never happen
                _ => throw new InvalidOperationException("Invalid effect enum provided"),
            };
    }
}
