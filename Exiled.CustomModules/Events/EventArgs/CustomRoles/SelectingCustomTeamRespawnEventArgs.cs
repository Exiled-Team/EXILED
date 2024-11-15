// -----------------------------------------------------------------------
// <copyright file="SelectingCustomTeamRespawnEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Events.EventArgs.Interfaces;
    using Respawning;

    /// <summary>
    /// Contains all information before selecting the next known team.
    /// </summary>
    public class SelectingCustomTeamRespawnEventArgs : IExiledEvent
    {
        private object team;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectingCustomTeamRespawnEventArgs" /> class.
        /// </summary>
        /// <param name="inTeam"><inheritdoc cref="Team"/></param>
        public SelectingCustomTeamRespawnEventArgs(object inTeam)
        {
            team = inTeam;
        }

        /// <summary>
        /// Gets or sets the next known team.
        /// </summary>
        public object Team
        {
            get => team;
            set
            {
                if (value == team)
                    return;

                if (value is SpawnableTeamType teamType)
                {
                    team = (object)teamType;
                    return;
                }

                if (CustomTeam.TryGet(value, out CustomTeam customTeam))
                {
                    team = (object)customTeam.Id;
                    return;
                }
            }
        }
    }
}