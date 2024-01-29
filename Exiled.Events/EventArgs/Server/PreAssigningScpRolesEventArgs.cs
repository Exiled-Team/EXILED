// -----------------------------------------------------------------------
// <copyright file="PreAssigningScpRolesEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before setting up the environment for the assignment of SCP roles.
    /// </summary>
    public class PreAssigningScpRolesEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreAssigningScpRolesEventArgs" /> class.
        /// </summary>
        /// <param name="targetScpNumber"><inheritdoc cref="Amount"/></param>
        public PreAssigningScpRolesEventArgs(int targetScpNumber) => Amount = targetScpNumber;

        /// <summary>
        /// Gets or sets the amount of SCPs to be spawned.
        /// </summary>
        public int Amount { get; set; }
    }
}