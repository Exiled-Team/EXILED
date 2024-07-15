// -----------------------------------------------------------------------
// <copyright file="SwingingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player swings a <see cref="Jailbird"/>.
    /// </summary>
    public class SwingingEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwingingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="swingItem">The item being swung.</param>
        /// <param name="isAllowed">Whether the item can be swung or not.</param>
        public SwingingEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase swingItem, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Item = Item.Get(swingItem);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's swinging an item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> that is being swung.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item can be swung.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
