// -----------------------------------------------------------------------
// <copyright file="ChangedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items;

    /// <summary>
    ///     Contains all information after a player's held item changes.
    /// </summary>
    public class ChangedItemEventArgs : IPlayerEvent, IItemEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangedItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldItem">
        ///     <inheritdoc cref="OldItem" />
        /// </param>
        public ChangedItemEventArgs(Player player, ItemBase oldItem)
        {
            Player = player;
            Item = Player.CurrentItem;
            OldItem = Item.Get(oldItem);
        }

        /// <summary>
        ///     Gets the previous item.
        /// </summary>
        public Item OldItem { get; }

        /// <summary>
        ///     Gets the new item.
        /// </summary>
        [Obsolete("Use ev.Item instead of this")]
        public Item NewItem => Item;

        /// <summary>
        ///     Gets the new item.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        ///     Gets the player who's changed the item.
        /// </summary>
        public Player Player { get; }
    }
}
