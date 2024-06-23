// -----------------------------------------------------------------------
// <copyright file="DanceType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All available dance variants being used by SCP-3114.
    /// </summary>
    public enum DanceType : byte
    {
        /// <summary>
        /// Break-dance.
        /// </summary>
        BreakDance,

        /// <summary>
        /// Chicken dance.
        /// </summary>
        ChickenDance,

        /// <summary>
        /// The "Running Man" dance.
        /// </summary>
        RunningMan,

        /// <summary>
        /// The "Maraschino" dance.
        /// </summary>
        Maraschino,

        /// <summary>
        /// Twist dance.
        /// </summary>
        Twist,

        /// <summary>
        /// The "Cabbage Patch" dance.
        /// </summary>
        CabbagePatch,

        /// <summary>
        /// Swing dance.
        /// </summary>
        Swing,

        /// <summary>
        /// Dance1
        /// </summary>
        None = byte.MaxValue,
    }
}