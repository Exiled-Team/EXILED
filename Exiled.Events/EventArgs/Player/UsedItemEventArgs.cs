// -----------------------------------------------------------------------
// <copyright file="UsedItemEventArgs.cs" company="Exiled Team">
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

    using InventorySystem.Items.Usables;

    /// <summary>
    ///     Contains all information after a player used an item.
    /// </summary>
    public class UsedItemEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UsedItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        ///     <inheritdoc cref="Item" />
        /// </param>
        public UsedItemEventArgs(ReferenceHub player, UsableItem item)
        {
            Player = Player.Get(player);
            Item = API.Features.Items.Item.Get(item) is Usable usable ? usable : null;
        }

        /// <summary>
        ///     Gets the item that the player used.
        /// </summary>
        public Usable Item { get; }

        /// <summary>
        ///     Gets the player who used the item.
        /// </summary>
        public Player Player { get; }
    }
}