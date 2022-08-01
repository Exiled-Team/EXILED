// -----------------------------------------------------------------------
// <copyright file="UsedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Interfaces.Item;

    using InventorySystem.Items.Usables;

    /// <summary>
    ///     Contains all information after a player used an item.
    /// </summary>
    public class UsedItemEventArgs : IPlayerEvent, IItemUsableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UsedItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        ///     <inheritdoc cref="Item" />
        /// </param>
        public UsedItemEventArgs(Player player, UsableItem item)
        {
            try
            {
                Player = player;
                Usable = item is null ? null : (Usable)Item.Get(item);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(UsedItemEventArgs)}.ctor: {e}");
            }
        }

        /// <summary>
        ///     Gets the item that the player used.
        /// </summary>
        public Usable Usable { get; }

        /// <summary>
        ///     Gets the player who used the item.
        /// </summary>
        public Player Player { get; }
    }
}
