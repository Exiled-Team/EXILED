// -----------------------------------------------------------------------
// <copyright file="AssigningHumanCustomRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before assigning human roles.
    /// </summary>
    public class AssigningHumanCustomRolesEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssigningHumanCustomRolesEventArgs" /> class.
        /// </summary>
        /// <param name="roles"><inheritdoc cref="Roles"/></param>
        public AssigningHumanCustomRolesEventArgs(List<object> roles) => Roles = roles;

        /// <summary>
        /// Gets or sets all roles to be assigned.
        /// </summary>
        public List<object> Roles { get; set; }
    }
}