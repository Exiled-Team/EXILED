// -----------------------------------------------------------------------
// <copyright file="AssigningScpRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using API.Features;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before assigning SCP roles.
    /// </summary>
    public class AssigningScpRolesEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssigningScpRolesEventArgs" /> class.
        /// </summary>
        /// <param name="chosenPlayers"><inheritdoc cref="Players"/></param>
        /// <param name="enqueuedScps"><inheritdoc cref="Roles"/></param>
        public AssigningScpRolesEventArgs(List<ReferenceHub> chosenPlayers, List<RoleTypeId> enqueuedScps)
        {
            Players = Player.Get(chosenPlayers).ToList();
            Roles = enqueuedScps;
        }

        /// <summary>
        /// Gets or sets the players to be spawned.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Gets or sets all roles to be assigned.
        /// </summary>
        public List<RoleTypeId> Roles { get; set; }

        /// <summary>
        /// Gets all chosen player's reference hubs.
        /// </summary>
        internal List<ReferenceHub> ChosenPlayers => Players.Select(player => player.ReferenceHub).ToList();
    }
}