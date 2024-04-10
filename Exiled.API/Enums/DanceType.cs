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
        /// Dance1
        /// </summary>
        Dance1,

        /// <summary>
        /// Dance2
        /// </summary>
        Dance2,

        /// <summary>
        /// Dance3
        /// </summary>
        Dance3,

        /// <summary>
        /// Dance4
        /// </summary>
        Dance4,

        /// <summary>
        /// Dance5
        /// </summary>
        Dance5,

        /// <summary>
        /// Dance1
        /// </summary>
        None = byte.MaxValue,
    }
}