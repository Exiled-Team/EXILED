// -----------------------------------------------------------------------
// <copyright file="SpawnableWave.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All spawnable factions and mini-waves available.
    /// </summary>
    public enum SpawnableWave
    {
        /// <summary>
        /// Represents a normal spawnable Ntf wave.
        /// </summary>
        NtfWave,

        /// <summary>
        /// Represents a spawnable smaller Ntf wave.
        /// </summary>
        NtfMiniWave,

        /// <summary>
        /// Represents a normal spawnable Chaos wave.
        /// </summary>
        ChaosWave,

        /// <summary>
        /// Represents a spawnable smaller Chaos wave.
        /// </summary>
        ChaosMiniWave,
    }
}