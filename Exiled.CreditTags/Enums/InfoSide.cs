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
        /// Checks if badge is available, if not checks if custom player info is available, if not it does not set anything
        /// </summary>
        FirstAvailable,

        /// <summary>
        /// Includes both of them.
        /// </summary>
        Both = Badge,
    }
}
