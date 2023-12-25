// -----------------------------------------------------------------------
// <copyright file="UsingTapeItemEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before player uses a Tape Item.
    /// </summary>
    public class UsingTapeItemEventArgs : IItemEvent, IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingTapeItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="itemBase"><inheritdoc cref="Item"/></param>
        /// <param name="success"><inheritdoc cref="Success"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingTapeItemEventArgs(Player player, ItemBase itemBase, bool success, bool isAllowed = true)
        {
            Player = player;
            TapeItem = Item.Get(itemBase).As<TapeItem>();
            Success = success;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Item Item => TapeItem;

        /// <summary>
        /// Gets a <see cref="API.Features.Items.TapeItem"/> which is being used.
        /// </summary>
        public TapeItem TapeItem { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not item has been successfully used.
        /// </summary>
        public bool Success { get; set; }
    }
}