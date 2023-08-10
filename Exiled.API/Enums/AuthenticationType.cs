// -----------------------------------------------------------------------
// <copyright file="AuthenticationType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Players authentication types.
    /// </summary>
    /// <seealso cref="Features.Player.AuthenticationType"/>
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
        [Obsolete("Value is unused.")]
        Patreon, // TODO: Removing this it's have never exist

        /// <summary>
        /// Indicates that the player has been authenticated through an unknown provider.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the player has been authenticated as localhost.
        /// </summary>
        LocalHost,

        /// <summary>
        /// Indicates that the player has been authenticated as DedicatedServer.
        /// </summary>
        DedicatedServer,
    }
}