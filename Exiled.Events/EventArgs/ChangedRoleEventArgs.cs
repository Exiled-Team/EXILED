// -----------------------------------------------------------------------
// <copyright file="ChangedRoleEventArgs.cs" company="Exiled Team">
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
    /// Contains all informations after player's <see cref="RoleType"/> changes.
    /// </summary>
    public class ChangedRoleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedRoleEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldRole"><inheritdoc cref="OldRole"/></param>
        /// <param name="items"><inheritdoc cref="Items"/></param>
        /// <param name="hasPreservedPosition"><inheritdoc cref="HasPreservedPosition"/></param>
        /// <param name="isEscaped"><inheritdoc cref="IsEscaped"/></param>
        public ChangedRoleEventArgs(Player player, RoleType oldRole, List<ItemType> items, bool hasPreservedPosition, bool isEscaped)
        {
            Player = player;
            OldRole = oldRole;
            Items = items.AsReadOnly();
            HasPreservedPosition = hasPreservedPosition;
            IsEscaped = isEscaped;
        }

        /// <summary>
        /// Gets the player whose <see cref="RoleType"/> has changed.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player's previous role.
        /// </summary>
        public RoleType OldRole { get; }

        /// <summary>
        /// Gets base items that the player has received.
        /// </summary>
        public IReadOnlyList<ItemType> Items { get; }

        /// <summary>
        /// Gets a value indicating whether the player escaped or not.
        /// </summary>
        public bool IsEscaped { get; }

        /// <summary>
        /// Gets a value indicating whether the position has been preserved after changing the role.
        /// </summary>
        public bool HasPreservedPosition { get; }
    }
}
