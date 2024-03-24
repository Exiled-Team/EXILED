// -----------------------------------------------------------------------
// <copyright file="DeployingScpRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Enums;
    using API.Features;

    using Interfaces;

    using PlayerRoles;
    using PlayerRoles.RoleAssign;

    /// <summary>
    /// Contains all information before deploying a SCP role.
    /// </summary>
    public class DeployingScpRoleEventArgs : IExiledEvent
    {
        private RoleTypeId role;
        private bool delegateHasChanged;
        private Action @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeployingScpRoleEventArgs"/> class.
        /// </summary>
        /// <param name="chosenPlayers">The players from which retrieve the player to be spawned from.</param>
        /// <param name="selectedRole"><inheritdoc cref="Role"/></param>
        /// <param name="enqueuedScps">All enqueued SCPs.</param>
        public DeployingScpRoleEventArgs(List<ReferenceHub> chosenPlayers, RoleTypeId selectedRole, List<RoleTypeId> enqueuedScps)
        {
            ChosenPlayers = chosenPlayers;
            Roles = enqueuedScps;
            Role = role;
            @delegate = () => ScpSpawner.AssignScp(chosenPlayers, selectedRole, enqueuedScps);
            OriginalDelegate = @delegate;
        }

        /// <summary>
        /// Gets or sets the <see langword="delegate"/> to be fired to deploy the role.
        /// </summary>
        public Action Delegate
        {
            get => @delegate;
            set
            {
                @delegate = value;
                delegateHasChanged = true;
            }
        }

        /// <summary>
        /// Gets the original <see langword="delegate"/> to be fired to deploy the role.
        /// </summary>
        public Action OriginalDelegate { get; }

        /// <summary>
        /// Gets or sets the role to be deployed.
        /// </summary>
        public RoleTypeId Role
        {
            get => role;
            set
            {
                if (role == value)
                    return;

                role = value;

                if (delegateHasChanged)
                    return;

                @delegate = () => ScpSpawner.AssignScp(ChosenPlayers, role, Roles);
            }
        }

        /// <summary>
        /// Gets all roles to be assigned.
        /// </summary>
        public List<RoleTypeId> Roles { get; }

        /// <summary>
        /// Gets all chosen player's reference hubs.
        /// </summary>
        internal List<ReferenceHub> ChosenPlayers { get; }
    }
}