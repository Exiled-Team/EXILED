// -----------------------------------------------------------------------
// <copyright file="AssigningHumanRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using API.Enums;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before assigning human roles.
    /// </summary>
    public class AssigningHumanRolesEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssigningHumanRolesEventArgs" /> class.
        /// </summary>
        /// <param name="roles"><inheritdoc cref="Roles"/></param>
        public AssigningHumanRolesEventArgs(RoleTypeId[] roles) => Roles = roles;

        /// <summary>
        /// Gets or sets all roles to be assigned.
        /// </summary>
        public RoleTypeId[] Roles { get; set; }
    }
}