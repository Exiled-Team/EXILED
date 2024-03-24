// -----------------------------------------------------------------------
// <copyright file="DeployingHumanRoleEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before deploying a human role.
    /// </summary>
    public class DeployingHumanRoleEventArgs : IExiledEvent
    {
        private RoleTypeId role;
        private bool delegateHasChanged;
        private Action @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeployingHumanRoleEventArgs" /> class.
        /// </summary>
        /// <param name="selectedRole"><inheritdoc cref="Role"/></param>
        public DeployingHumanRoleEventArgs(RoleTypeId selectedRole)
        {
            Role = selectedRole;
            @delegate = () => HumanSpawner.AssignHumanRoleToRandomPlayer(selectedRole);
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

                @delegate = () => HumanSpawner.AssignHumanRoleToRandomPlayer(role);
            }
        }
    }
}