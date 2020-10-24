// -----------------------------------------------------------------------
// <copyright file="PickingUpScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player picks up an item.
    /// </summary>
    public class PickingUpScp330EventArgs : ItemDroppedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpScp330EventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's picking up the item.</param>
        /// <param name="pickup">The pickup to be picked up.</param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PickingUpScp330EventArgs(Player player, Pickup pickup, bool isAllowed = true)
            : base(player, pickup)
        {
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this pickup should be severe or not.
        /// </summary>
        public bool IsSevere { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what item will be picked up.
        /// </summary>
        public ItemType ItemId { get; set; }
    }
}
