// -----------------------------------------------------------------------
// <copyright file="UsingItemCompletedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information before a player uses an item.
    /// </summary>
    public class UsingItemCompletedEventArgs : IPlayerEvent, IDeniableEvent, IUsableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingItemCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's going to use the item.</param>
        /// <param name="item">
        /// <inheritdoc cref="UsedItemEventArgs.Item" />
        /// </param>
        public UsingItemCompletedEventArgs(Player player, UsableItem item)
        {
            Player = player;
            Usable = Item.Get(item) is Usable usable ? usable : null;
        }

        /// <summary>
        /// Gets the item that the player using.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        /// Gets the player who using the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can use the item.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}