// -----------------------------------------------------------------------
// <copyright file="GeneratorState.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Enums
{
    using System;

    /// <summary>
    /// Generator states.
    /// </summary>
    [Flags]
    public enum GeneratorState : byte
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 1,

        /// <summary>
        /// Generator is unlocked.
        /// </summary>
        Unlocked = 2,

        /// <summary>
        /// Generator is open.
        /// </summary>
        Open = 4,

        /// <summary>
        /// Generator is activating.
        /// </summary>
        Activating = 8,

        /// <summary>
        /// Generator is engaged.
        /// </summary>
        Engaged = 16,
    }
}
