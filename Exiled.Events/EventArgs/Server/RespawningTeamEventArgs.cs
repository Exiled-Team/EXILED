// -----------------------------------------------------------------------
// <copyright file="RespawningTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerRoles;

    using Respawning;

    /// <summary>
    /// Contains all information before spawning a wave of <see cref="SpawnableTeamType.NineTailedFox" /> or
    /// <see cref="SpawnableTeamType.ChaosInsurgency" />.
    /// </summary>
    public class RespawningTeamEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RespawningTeamEventArgs"/> class.
        /// </summary>
        /// <param name="players"><inheritdoc cref="Players"/></param>
        /// <param name="maxRespawn"><inheritdoc cref="MaxWaveSize"/></param>
        /// <param name="nextKnownTeam"><inheritdoc cref="NextKnownTeam"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public RespawningTeamEventArgs(List<Player> players, int maxRespawn, SpawnableTeamType nextKnownTeam, bool isAllowed = true)
        {
            Players = players;
            MaxWaveSize = maxRespawn;
            NextKnownTeam = nextKnownTeam;
            SpawnQueue = new();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the list of players that are going to be respawned.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        /// Gets a value the next team to be respawned.
        /// </summary>
        public SpawnableTeamType NextKnownTeam { get; }

        /// <summary>
        /// Gets the maximum amount of respawnable players.
        /// </summary>
        public int MaxWaveSize { get; }

        /// <summary>
        /// Gets or sets the RoleTypeId spawn queue.
        /// </summary>
        public Queue<RoleTypeId> SpawnQueue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
