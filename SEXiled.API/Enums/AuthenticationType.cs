// -----------------------------------------------------------------------
// <copyright file="AuthenticationType.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Enums
{
    /// <summary>
    /// Players authentication types.
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// Indicates that the player has been authenticated through Steam.
        /// </summary>
        Steam,

        /// <summary>
        /// Indicates that the player has been authenticated through Discord.
        /// </summary>
        Discord,

        /// <summary>
        /// Indicates that the player has been authenticated as a Northwood staffer.
        /// </summary>
        Northwood,

        /// <summary>
        /// Indicates that the player has been authenticated as a Patreon.
        /// </summary>
        Patreon,

        /// <summary>
        /// Indicates that the player has been authenticated through an unknown provider.
        /// </summary>
        Unknown,
    }
}
