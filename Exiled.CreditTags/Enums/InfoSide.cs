// -----------------------------------------------------------------------
// <copyright file="InfoSide.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Enums
{
    public enum InfoSide
    {
        /// <summary>
        /// Uses badge.
        /// </summary>
        Badge,

        /// <summary>
        /// Uses Custom Player Info area
        /// </summary>
        CustomPlayerInfo,

        /// <summary>
        /// Uses Badge if available, otherwise uses CustomPlayerInfo if available.
        /// </summary>
        FirstAvailable,

        /// <summary>
        /// Includes both options.
        /// </summary>
        Both = Badge,
    }
}