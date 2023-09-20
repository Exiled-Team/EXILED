// -----------------------------------------------------------------------
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

    /// <summary>
    /// Contains all information after team respawns.
    /// </summary>
    public class RespawnedTeamEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnedTeamEventArgs"/> class.
        /// </summary>
        /// <param name="players"><inheritdoc cref="Players"/></param>
        /// <param name="spawnedTeam"><inheritdoc cref="SpawnedTeam"/></param>
        public RespawnedTeamEventArgs(IEnumerable<ReferenceHub> players, SpawnableTeamType spawnedTeam)
        {
            Players = players.Select(Player.Get).ToList();
            SpawnedTeam = spawnedTeam;
        }

        /// <summary>
        /// Gets the list of players that have been spawned.
        /// </summary>
        public IReadOnlyCollection<Player> Players { get; }

        /// <summary>
        /// Gets the spawned team's <see cref="SpawnableTeamType"/>.
        /// </summary>
        public SpawnableTeamType SpawnedTeam { get; }
    }
}