// -----------------------------------------------------------------------
// <copyright file="RespawnEffectType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Layers game respawn effects.
    /// </summary>
    public enum RespawnEffectType : byte
    {
        /// <summary>
        /// Play the chaos music to alive Class-D and Chaos.
        /// </summary>
        PlayChaosMusic = 0,

        /// <summary>
        /// Summon the Chaos van
        /// </summary>
        SummonChaosVan = 128,

        /// <summary>
        /// Summon the NTF chopper.
        /// </summary>
        SummonNtfChopper = 129,
    }
}
