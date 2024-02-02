// -----------------------------------------------------------------------
// <copyright file="PreRespawningTeamEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before setting up the environment for respawning a wave of <see cref="SpawnableTeamType.NineTailedFox" /> or
    /// <see cref="SpawnableTeamType.ChaosInsurgency" />.
    /// </summary>
    public class PreRespawningTeamEventArgs : IDeniableEvent
    {
        private SpawnableTeamType nextKnownTeam;
        private int maxWaveSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreRespawningTeamEventArgs"/> class.
        /// </summary>
        /// <param name="spawnableTeamHandler"><inheritdoc cref="SpawnableTeamHandler"/></param>
        /// <param name="maxRespawn"><inheritdoc cref="MaxWaveSize"/></param>
        /// <param name="nextKnownTeam"><inheritdoc cref="NextKnownTeam"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PreRespawningTeamEventArgs(SpawnableTeamHandlerBase spawnableTeamHandler, int maxRespawn, SpawnableTeamType nextKnownTeam, bool isAllowed = true)
        {
            SpawnableTeamHandler = spawnableTeamHandler;
            MaxWaveSize = maxRespawn;
            this.nextKnownTeam = nextKnownTeam;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpawnableTeamHandlerBase"/>.
        /// </summary>
        public SpawnableTeamHandlerBase SpawnableTeamHandler { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of respawnable players.
        /// </summary>
        public int MaxWaveSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what the next respawnable team is.
        /// </summary>
        public SpawnableTeamType NextKnownTeam
        {
            get => nextKnownTeam;
            set
            {
                nextKnownTeam = value;

                if (!RespawnManager.SpawnableTeams.TryGetValue(value, out SpawnableTeamHandlerBase spawnableTeam))
                {
                    MaxWaveSize = 0;
                    return;
                }

                MaxWaveSize = spawnableTeam.MaxWaveSize;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
