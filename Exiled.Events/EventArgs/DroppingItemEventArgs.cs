// -----------------------------------------------------------------------
// <copyright file="DroppingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player drops an item.
    /// </summary>
    public class DroppingItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DroppingItemEventArgs(Player player, Inventory.SyncItemInfo item, bool isAllowed = true)
        {
            Player = player;
            Item = item;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's dropping the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item to be dropped.
        /// </summary>
        public Inventory.SyncItemInfo Item { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the item can be dropped.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
