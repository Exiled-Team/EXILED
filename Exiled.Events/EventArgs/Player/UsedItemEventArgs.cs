// -----------------------------------------------------------------------
// <copyright file="UsedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Usables;

    /// <summary>
    ///     Contains all information after a player used an item.
    /// </summary>
    public class UsedItemEventArgs : IPlayerEvent, IUsableEvent
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
            Usable = Item.Get(item) is Usable usable ? usable : null;
        }

        /// <summary>
        ///     Gets the item that the player used.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        ///     Gets the player who used the item.
        /// </summary>
        public Player Player { get; }
    }
}