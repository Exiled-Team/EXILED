// -----------------------------------------------------------------------
// <copyright file="InfoSide.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CreditTags.Enums
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
        /// Includes both of them.
        /// </summary>
        Both = Badge,
    }
}
