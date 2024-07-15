// -----------------------------------------------------------------------
// <copyright file="ChargingJailbirdEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before a <see cref="API.Features.Items.Jailbird"/> charged attack.
    /// </summary>
    public class ChargingJailbirdEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingJailbirdEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="swingItem">The item being charged.</param>
        /// <param name="isAllowed">Whether the item can be charged or not.</param>
        public ChargingJailbirdEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase swingItem, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Item = Item.Get(swingItem);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's charging an item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> that is being charged. This will always be a <see cref="Jailbird"/>.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the Jailbird can be charged.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
