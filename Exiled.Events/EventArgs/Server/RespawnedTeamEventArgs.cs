﻿// -----------------------------------------------------------------------
// <copyright file="RespawnedTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning;
    using Respawning.Waves;

    /// <summary>
    /// Contains all information after a team has spawned.
    /// </summary>
    public class RespawnedTeamEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnedTeamEventArgs"/> class.
        /// </summary>
        /// <param name="hubs"><inheritdoc cref="Players"/></param>
        /// <param name="team"><inheritdoc cref="Team"/></param>
        public RespawnedTeamEventArgs(SpawnableWaveBase team, IEnumerable<ReferenceHub> hubs)
        {
            Players = hubs.Select(Player.Get);
            Team = team;
        }

        /// <summary>
        /// Gets all spawned players.
        /// </summary>
        public IEnumerable<Player> Players { get; }

        /// <summary>
        /// Gets the spawned team.
        /// </summary>
        public SpawnableWaveBase Team { get; }
    }
}