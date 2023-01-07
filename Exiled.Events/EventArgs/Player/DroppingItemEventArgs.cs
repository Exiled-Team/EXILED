// -----------------------------------------------------------------------
// <copyright file="DroppingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items;

    using PlayerRoles;

    /// <summary>
    ///     Contains all information before a player drops an item.
    /// </summary>
    public class DroppingItemEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        private bool isAllowed = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DroppingItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        ///     <inheritdoc cref="Item" />
        /// </param>
        /// <param name="isThrown">
        ///     <inheritdoc cref="IsThrown" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DroppingItemEventArgs(Player player, ItemBase item, bool isThrown, bool isAllowed = true)
        {
            Player = player;
            Item = Item.Get(item);
            IsAllowed = isAllowed;
            IsThrown = isThrown;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item was thrown.
        /// </summary>
        public bool IsThrown { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item can be dropped.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                if (Player.Role == RoleTypeId.Spectator)
                    isAllowed = true;
                return isAllowed;
            }

            set
            {
                if (Player.Role == RoleTypeId.Spectator)
                    value = true;
                isAllowed = value;
            }
        }

        /// <summary>
        ///     Gets the item to be dropped.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        ///     Gets the player who's dropping the item.
        /// </summary>
        public Player Player { get; }
    }
}