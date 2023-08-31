// -----------------------------------------------------------------------
// <copyright file="CancelledItemUseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Usables;

    /// <summary>
    ///     Contains all information before a player cancels usage of an item.
    /// </summary>
    public class CancelledItemUseEventArgs : IPlayerEvent, IItemEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CancelledItemUseEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's stopping the use of an item.</param>
        /// <param name="item">
        ///     <inheritdoc cref="UsedItemEventArgs.Item" />
        /// </param>
        public CancelledItemUseEventArgs(Player player, UsableItem item)
        {
            Player = player;
            Usable = Item.Get(item) is Usable usable ? usable : null;
        }

        /// <summary>
        ///     Gets the item that the player cancelling.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        ///     Gets the player who cancelling the item.
        /// </summary>
        public Player Player { get; }
    }
}