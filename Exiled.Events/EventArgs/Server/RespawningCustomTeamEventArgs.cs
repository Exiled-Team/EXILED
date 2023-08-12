// -----------------------------------------------------------------------
// <copyright file="RespawnCustomTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before spawning a custom wave.
    /// </summary>
    public class RespawningCustomTeamEventArgs : IExiledEvent
    {
        private int maximumRespawnAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawningCustomTeamEventArgs"/> class.
        /// </summary>
        /// <param name="players">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="maxRespawn">
        ///     <inheritdoc cref="MaximumRespawnAmount" />
        /// </param>
        /// <param name="teamId">
        ///     The team id that was returned by the <see cref="Exiled.Events.Handlers.Server.SelectTeam"/>.
        /// </param>
        public RespawningCustomTeamEventArgs(List<Player> players, int maxRespawn, uint teamId)
        {
            Players = players;
            MaximumRespawnAmount = maxRespawn;
            TeamId = teamId;
        }

        /// <summary>
        ///     Gets or sets the maximum amount of respawnable players.
        /// </summary>
        public int MaximumRespawnAmount
        {
            get => maximumRespawnAmount;
            set
            {
                if (value < maximumRespawnAmount)
                {
                    if (Players.Count > value)
                        Players.RemoveRange(value, Players.Count - value);
                }

                maximumRespawnAmount = value;
            }
        }

        /// <summary>
        ///     Gets the list of players that are going to be respawned.
        /// </summary>
        public List<Player> Players { get; }

        /// <summary>
        ///     Gets the team id curently respawing.
        /// </summary>
        public uint TeamId { get; }
    }
}