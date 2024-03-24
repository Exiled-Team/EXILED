// -----------------------------------------------------------------------
// <copyright file="ChangedCustomRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using System.Collections.Generic;

    using API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;

    /// <summary>
    /// Contains all information after a player changes role to a custom role.
    /// </summary>
    public class ChangedCustomRoleEventArgs : IExiledEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedCustomRoleEventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        public ChangedCustomRoleEventArgs(Player player, object role)
        {
            Player = player;
            Role = role;
        }

        /// <summary>
        /// Gets the player who changed role.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the previous role.
        /// </summary>
        public object Role { get; }
    }
}