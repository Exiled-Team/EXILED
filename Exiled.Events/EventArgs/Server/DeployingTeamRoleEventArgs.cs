// -----------------------------------------------------------------------
// <copyright file="DeployingTeamRoleEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before deploying a team role.
    /// </summary>
    public class DeployingTeamRoleEventArgs : IExiledEvent, IPlayerEvent
    {
        private RoleTypeId role;
        private bool delegateHasChanged;
        private Action @delegate;
        private Player player;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeployingTeamRoleEventArgs"/> class.
        /// </summary>
        /// <param name="hub"><inheritdoc cref="Player"/></param>
        /// <param name="selectedRole"><inheritdoc cref="Role"/></param>
        public DeployingTeamRoleEventArgs(ReferenceHub hub, RoleTypeId selectedRole)
        {
            Player = Player.Get(hub);
            Role = selectedRole;
            @delegate = () => hub.roleManager.ServerSetRole(selectedRole, RoleChangeReason.Respawn);
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
        /// Gets or sets the player to be deployed.
        /// </summary>
        public Player Player
        {
            get => player;
            set
            {
                if (player == value)
                    return;

                player = value;

                if (delegateHasChanged)
                    return;

                @delegate = () => Player.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.Respawn);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the role's deployment is reliable.
        /// <para/>
        /// When set to <see langword="false"/>, the <see cref="Player"/> will not be counted among respawned players.
        /// </summary>
        public bool IsReliable { get; set; } = true;

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

                @delegate = () => Player.ReferenceHub.roleManager.ServerSetRole(role, RoleChangeReason.Respawn);
            }
        }
    }
}