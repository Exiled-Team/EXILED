// -----------------------------------------------------------------------
// <copyright file="Effect.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    public class Effect
    {
        private static List<Effect> effects;

        public static IReadOnlyList<Effect> AllEffects
        {
            get
            {
                if (effects is null)
                {
                    foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
                    {

                    }
                }

                return effects.AsReadOnly();
            }
        }

        public string Name => Type.Name;

        public EffectType EffectType { get; }

        public Type Type { get; }

        private Effect(EffectType effectType)
        {
            EffectType = effectType;
            Type = EffectType.Type();
        }
    }
}
