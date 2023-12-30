// -----------------------------------------------------------------------
// <copyright file="UsingTapeEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before player uses a tape.
    /// </summary>
    public class UsingTapeEventArgs : IItemEvent, IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingTapeEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="itemBase"><inheritdoc cref="Item"/></param>
        /// <param name="success"><inheritdoc cref="Success"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingTapeEventArgs(Player player, ItemBase itemBase, bool success, bool isAllowed = true)
        {
            Player = player;
            TapeItem = Item.Get(itemBase).As<TapeItem>();
            Success = success;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Item Item => TapeItem;

        /// <summary>
        /// Gets the <see cref="API.Features.Items.TapeItem"/> which is being used.
        /// </summary>
        public TapeItem TapeItem { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has been successfully used.
        /// </summary>
        public bool Success { get; set; }
    }
}