// -----------------------------------------------------------------------
// <copyright file="PickingUpItemEventArgs.cs" company="Exiled Team">
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
    public class PickingUpItemEventArgs : ItemDroppedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpItemEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's picking up the item.</param>
        /// <param name="pickup">The pickup to be picked up.</param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PickingUpItemEventArgs(Player player, Pickup pickup, bool isAllowed = true)
            : base(player, pickup)
        {
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the item can be picked up.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
