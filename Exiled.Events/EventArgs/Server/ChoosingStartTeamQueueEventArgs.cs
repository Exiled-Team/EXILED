// -----------------------------------------------------------------------
// <copyright file="ChoosingStartTeamQueueEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Exiled.API.Features.Pools;
    using Interfaces;
    using PlayerRoles;

    /// <summary>
    /// Contains all information before a spectator changes the spectated player.
    /// </summary>
    public class ChoosingStartTeamQueueEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoosingStartTeamQueueEventArgs" /> class.
        /// </summary>
        /// <param name="teamRespawnQueue">
        /// <inheritdoc cref="TeamRespawnQueue" />
        /// </param>
        public ChoosingStartTeamQueueEventArgs(string teamRespawnQueue)
        {
            TeamRespawnQueue = new();
            foreach (char ch in teamRespawnQueue)
            {
                Team team = (Team)(ch - '0');
                if (Enum.IsDefined(typeof(Team), team))
                    TeamRespawnQueue.Add(team);
            }
        }

        /// <summary>
        /// Gets the TeamRespawnQueue.
        /// </summary>
        public List<Team> TeamRespawnQueue { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the TeamRespawnQueue in a string value.
        /// </summary>
        /// <returns>The actual modified TeamRespawnQueue.</returns>
        internal string GetTeamRespawnQueue()
        {
            StringBuilder teamRespawnQueue = StringBuilderPool.Pool.Get();

            foreach (Team team in TeamRespawnQueue)
                teamRespawnQueue.Append((int)team);

            return StringBuilderPool.Pool.ToStringReturn(teamRespawnQueue);
        }
    }
}