// -----------------------------------------------------------------------
// <copyright file="RoundEndedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information after the end of a round.
    /// </summary>
    public class SearchPickupRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPickupRequestEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="searchTime"><inheritdoc cref="SearchTime"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>

        public SearchPickupRequestEventArgs(Player player, ItemPickupBase pickup, float searchTime, bool isAllowed)
        {
            Player = player;
            Pickup = Pickup.Get(pickup);
            SearchTime = searchTime;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the Player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the searching Pickup.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the Pickup can be search.
        /// </summary>
        public float SearchTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Pickup can be search.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
