// -----------------------------------------------------------------------
// <copyright file="UsedItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information after a player used an item.
    /// </summary>
    public class UsedItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsedItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        public UsedItemEventArgs(Player player, UsableItem item)
        {
            try
            {
                Player = player;
                Item = item == null ? null : (Usable)API.Features.Items.Item.Get(item);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(UsedItemEventArgs)}.ctor: {e}");
            }
        }

        /// <summary>
        /// Gets the player who used the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the item that the player used.
        /// </summary>
        public Usable Item { get; }
    }
}
