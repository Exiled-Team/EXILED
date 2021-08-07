// -----------------------------------------------------------------------
// <copyright file="DroppingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before a player drops an item.
    /// </summary>
    public class DroppingItemEventArgs : EventArgs
    {
        private bool isAllowed = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="serial">The Serial number of the item.</param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DroppingItemEventArgs(Player player, ushort serial, bool isAllowed = true)
        {
            Player = player;
            Item = player.Inventory.UserInventory.Items.TryGetValue(serial, out ItemBase pickupBase) ? Item.Get(pickupBase) : null;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's dropping the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item to be dropped.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the item can be dropped.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                if (Player.Role == RoleType.Spectator)
                    isAllowed = true;
                return isAllowed;
            }

            set
            {
                if (Player.Role == RoleType.Spectator)
                    value = true;
                isAllowed = value;
            }
        }
    }
}
