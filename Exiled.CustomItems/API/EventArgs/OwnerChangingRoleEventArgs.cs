// -----------------------------------------------------------------------
// <copyright file="OwnerChangingRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Contains all informations of a <see cref="CustomItem"/> before a <see cref="Player"/> escapes.
    /// </summary>
    public class OwnerChangingRoleEventArgs : ChangingRoleEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        public OwnerChangingRoleEventArgs(Inventory.SyncItemInfo item, ChangingRoleEventArgs ev)
            : this(item, ev.Player, ev.NewRole, ev.Items, ev.ShouldPreservePosition, ev.IsEscaped)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="player"><inheritdoc cref="ChangingRoleEventArgs.Player"/></param>
        /// <param name="newRole"><inheritdoc cref="ChangingRoleEventArgs.NewRole"/></param>
        /// <param name="items"><inheritdoc cref="ChangingRoleEventArgs.Items"/></param>
        /// <param name="shouldPreservePosition"><inheritdoc cref="ChangingRoleEventArgs.ShouldPreservePosition"/></param>
        /// <param name="isEscaped"><inheritdoc cref="ChangingRoleEventArgs.IsEscaped"/></param>
        public OwnerChangingRoleEventArgs(Inventory.SyncItemInfo item, Player player, RoleType newRole, List<ItemType> items, bool shouldPreservePosition, bool isEscaped)
            : base(player, newRole, items, shouldPreservePosition, isEscaped)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the <see cref="Inventory.SyncItemInfo"/> as a <see cref="CustomItem"/> in the player's inventory.
        /// </summary>
        public Inventory.SyncItemInfo Item { get; }
    }
}
