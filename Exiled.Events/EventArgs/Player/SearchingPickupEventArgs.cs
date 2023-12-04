// -----------------------------------------------------------------------
// <copyright file="SearchingPickupEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    /// <summary>
    ///     Contains all information before a player searches a Pickup.
    /// </summary>
    public class SearchingPickupEventArgs : IPlayerEvent, IPickupEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchingPickupEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="searchSession">
        ///     <inheritdoc cref="SearchSession" />
        /// </param>
        /// <param name="searchCompletor">
        ///     <inheritdoc cref="SearchCompletor" />
        /// </param>
        /// <param name="searchTime">
        ///     <inheritdoc cref="SearchTime" />
        /// </param>
        public SearchingPickupEventArgs(Player player, ItemPickupBase pickup, SearchSession searchSession, SearchCompletor searchCompletor, float searchTime)
        {
            Player = player;
            Pickup = Pickup.Get(pickup);
            SearchSession = searchSession;
            SearchCompletor = searchCompletor;
            SearchTime = searchTime;
        }

        /// <summary>
        ///     Gets or sets the SearchSession.
        /// </summary>
        public SearchSession SearchSession { get; set; }

        /// <summary>
        ///     Gets or sets the SearchCompletor.
        /// </summary>
        public SearchCompletor SearchCompletor { get; set; }

        /// <summary>
        ///     Gets or sets the Pickup search duration.
        /// </summary>
        public float SearchTime { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Pickup can be searched.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the Pickup that is being searched.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        ///     Gets the Player who's searching the Pickup.
        /// </summary>
        public Player Player { get; }
    }
}