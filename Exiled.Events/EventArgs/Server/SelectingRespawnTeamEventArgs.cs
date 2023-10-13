// -----------------------------------------------------------------------
// <copyright file="SelectingRespawnTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;

    using Exiled.API.Features;

    using Respawning;

    /// <summary>
    /// Contains all information before selecting the team to respawn next.
    /// </summary>
    public class SelectingRespawnTeamEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectingRespawnTeamEventArgs"/> class.
        /// </summary>
        /// <param name="type">The <see cref="SpawnableTeamType"/> used as the starting value for this event.</param>
        public SelectingRespawnTeamEventArgs(SpawnableTeamType type)
        {
            ChosenTeam = type;
        }

        /// <summary>
        /// Gets or sets <see cref="SpawnableTeamType"/> that represents the team chosen to spawn.
        /// </summary>
        public SpawnableTeamType ChosenTeam { get; set; }
    }
}