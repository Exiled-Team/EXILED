// -----------------------------------------------------------------------
// <copyright file="ChangingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player's held item changes.
    /// </summary>
    public class ChangingItemEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldItem"><inheritdoc cref="OldItem"/></param>
        /// <param name="newItem"><inheritdoc cref="NewItem"/></param>
        public ChangingItemEventArgs(Player player, Inventory.SyncItemInfo oldItem, Inventory.SyncItemInfo newItem)
        {
            Player = player;
            OldItem = oldItem;
            NewItem = newItem;
        }

        /// <summary>
        /// Gets the player who's changing the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the old item.
        /// </summary>
        public Inventory.SyncItemInfo OldItem { get; set; }

        /// <summary>
        /// Gets the new item.
        /// </summary>
        public Inventory.SyncItemInfo NewItem { get; }
    }
}
