// -----------------------------------------------------------------------
// <copyright file="ServerHandlers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Events
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Server;
    using PlayerRoles;

    /// <summary>
    ///  Handles general events for server.
    /// </summary>
    public class ServerHandlers
    {
        private readonly CustomRoles plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="CustomRoles"/> plugin instance.</param>
        public ServerHandlers(CustomRoles plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.EndingRound"/>
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!plugin.CheckRoundEnd)
                return;

            HashSet<CustomTeam> customTeams = new();
            HashSet<Team> teams = new();

            foreach (Player player in Player.List)
            {
                var playerTeams = player.GetCustomTeams();
                if (playerTeams.Count == 0)
                {
                    teams.Add(player.Role.Team);
                    continue;
                }

                foreach (CustomTeam team in playerTeams)
                    customTeams.Add(team);
            }

            foreach (CustomTeam team in customTeams)
            {
                if (team.EnemyFaction.Any(p => teams.Contains(p)))
                {
                    ev.IsRoundEnded = false;
                    return;
                }

                if (customTeams.Any(p => team.EnemyCustomsTeamsId.Contains(p.Id)))
                {
                    ev.IsRoundEnded = false;
                    return;
                }
            }

            bool first = true;
            Faction faction = Faction.Unclassified;
            foreach (Team team in teams)
            {
                if (first)
                {
                    first = false;
                    faction = team.GetFaction();
                    continue;
                }

                if (faction != team.GetFaction())
                {
                    ev.IsRoundEnded = false;
                    return;
                }
            }

            ev.IsRoundEnded = true;
        }
    }
}
