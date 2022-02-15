// -----------------------------------------------------------------------
// <copyright file="CancellingItemUseEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Features;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information before a player cancels usage of a medical item.
    /// </summary>
    public class CancellingItemUseEventArgs : UsingItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellingItemUseEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's stopping the use of the medical item.</param>
        /// <param name="item"><inheritdoc cref="UsedItemEventArgs.Item"/></param>
        public CancellingItemUseEventArgs(Player player, UsableItem item)
            : base(player, item, 0)
        {
        }
    }
}
