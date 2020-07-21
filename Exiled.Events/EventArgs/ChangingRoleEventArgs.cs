// -----------------------------------------------------------------------
// <copyright file="ChangingRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player changes his role.
    /// </summary>
    public class ChangingRoleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newRole"><inheritdoc cref="NewRole"/></param>
        /// <param name="items"><inheritdoc cref="Items"/></param>
        /// <param name="shouldPreservePosition"><inheritdoc cref="ShouldPreservePosition"/></param>
        /// <param name="isEscaped"><inheritdoc cref="IsEscaped"/></param>
        public ChangingRoleEventArgs(Player player, RoleType newRole, List<ItemType> items, bool shouldPreservePosition, bool isEscaped)
        {
            Player = player;
            NewRole = newRole;
            Items = items;
            ShouldPreservePosition = shouldPreservePosition;
            IsEscaped = isEscaped;
        }

        /// <summary>
        /// Gets the player who'll change his role.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the new player's role.
        /// </summary>
        public RoleType NewRole { get; set; }

        /// <summary>
        /// Gets or sets base items that the player will receive.
        /// </summary>
        public List<ItemType> Items { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is escaped or not.
        /// </summary>
        public bool IsEscaped { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the position has to be preserved after changing the role.
        /// </summary>
        public bool ShouldPreservePosition { get; set; }
    }
}
