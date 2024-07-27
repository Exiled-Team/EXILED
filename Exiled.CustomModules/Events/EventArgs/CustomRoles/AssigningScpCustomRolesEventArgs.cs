// -----------------------------------------------------------------------
// <copyright file="AssigningScpCustomRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before assigning SCP roles.
    /// </summary>
    public class AssigningScpCustomRolesEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssigningScpCustomRolesEventArgs" /> class.
        /// </summary>
        /// <param name="chosenPlayers"><inheritdoc cref="Players"/></param>
        /// <param name="enqueuedScps"><inheritdoc cref="Roles"/></param>
        public AssigningScpCustomRolesEventArgs(List<ReferenceHub> chosenPlayers, List<object> enqueuedScps)
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
        public List<object> Roles { get; set; }
    }
}