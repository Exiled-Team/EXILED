// -----------------------------------------------------------------------
// <copyright file="PickingUpScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player interacts with SCP-330.
    /// </summary>
    public class PickingUpScp330EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpScp330EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="usage"><inheritdoc cref="Usage"/></param>
        /// <param name="item"><inheritdoc cref="ItemId"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PickingUpScp330EventArgs(Player player, int usage, ItemType item, bool isAllowed = true)
        {
            Player = player;
            Usage = usage;
            IsSevere = usage > 2;
            ItemId = item;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets player's pickup counter.
        /// </summary>
        public int Usage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pickup should be severe.
        /// </summary>
        public bool IsSevere { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what item will be picked up.
        /// </summary>
        public ItemType ItemId { get; set; }
    }
}
