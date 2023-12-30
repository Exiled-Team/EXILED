// -----------------------------------------------------------------------
// <copyright file="UsedTapeItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items;

    /// <summary>
    /// Contains all information after player uses tape.
    /// </summary>
    public class UsedTapeItemEventArgs : IItemEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsedTapeItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="itemBase"><inheritdoc cref="Item"/></param>
        public UsedTapeItemEventArgs(Player player, ItemBase itemBase)
        {
            Player = player;
            TapeItem = Item.Get(itemBase).As<TapeItem>();
        }

        /// <inheritdoc/>
        public Item Item => TapeItem;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.TapeItem"/> which was used.
        /// </summary>
        public TapeItem TapeItem { get; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}