// -----------------------------------------------------------------------
// <copyright file="ChangingCustomRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomRoles
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player changes role to a custom role.
    /// </summary>
    public class ChangingCustomRoleEventArgs : IExiledEvent, IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingCustomRoleEventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingCustomRoleEventArgs(Player player, object role, bool isAllowed = true)
        {
            Player = player;
            Role = role;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the player who's changing role.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the role to be changed to.
        /// <para/>
        /// Supports both roles and custom roles.
        /// </summary>
        public object Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can change role.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}