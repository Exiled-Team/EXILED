// -----------------------------------------------------------------------
// <copyright file="AssigningHumanRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using System.Collections.Generic;

    using API.Enums;
    using Exiled.Events.EventArgs.Interfaces;
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
        public AssigningHumanRolesEventArgs(List<object> roles) => Roles = roles;

        /// <summary>
        /// Gets or sets all roles to be assigned.
        /// </summary>
        public List<object> Roles { get; set; }
    }
}