// -----------------------------------------------------------------------
// <copyright file="GeneratorState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Features;

    /// <summary>
    /// Represents the state of a <see cref="Generator"/>.
    /// </summary>
    /// <seealso cref="Generator.State"/>
    /// <seealso cref="Generator.Get(GeneratorState)"/>
    [Flags]
    public enum GeneratorState : byte
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 1 << 0,

        /// <summary>
        /// Generator is unlocked.
        /// </summary>
        Unlocked = 1 << 1,

        /// <summary>
        /// Generator is open.
        /// </summary>
        Open = 1 << 2,

        /// <summary>
        /// Generator is activating.
        /// </summary>
        Activating = 1 << 3,

        /// <summary>
        /// Generator is engaged.
        /// </summary>
        Engaged = 1 << 4,
    }
}