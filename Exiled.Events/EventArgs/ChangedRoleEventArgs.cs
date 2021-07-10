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
    using System.Linq;

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
        /// <param name="oldCufferId"><inheritdoc cref="OldCufferId"/></param>
        /// <param name="hasPreservedPosition"><inheritdoc cref="HasPreservedPosition"/></param>
        /// <param name="isEscaped"><inheritdoc cref="IsEscaped"/></param>
        public ChangedRoleEventArgs(Player player, RoleType oldRole, int oldCufferId, bool hasPreservedPosition, bool isEscaped)
        {
            Player = player;
            OldRole = oldRole;
            OldCufferId = oldCufferId;
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
        /// Gets the id of the player's previous cuffer, or -1 if they were not cuffed.
        /// </summary>
        public int OldCufferId { get; }

        /// <summary>
        /// Gets items that the player had before their role was changed.
        /// </summary>
        [Obsolete("Use Player.Items instead.", true)]
        public IReadOnlyList<ItemType> Items => Player.Items.Select(item => item.id).ToList().AsReadOnly();

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
