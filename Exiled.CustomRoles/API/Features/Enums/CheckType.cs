// -----------------------------------------------------------------------
// <copyright file="CheckType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Enums
{
    /// <summary>
    /// The possible types of checks to preform on active abilities.
    /// </summary>
    public enum CheckType
    {
        /// <summary>
        /// Check if the ability is available to the player. (DOES NOT CHECK COOLDOWNS)
        /// </summary>
        Available,

        /// <summary>
        /// Check if the ability is selected, but not active.
        /// </summary>
        Selected,

        /// <summary>
        /// The ability is currently active.
        /// </summary>
        Active,
    }
}