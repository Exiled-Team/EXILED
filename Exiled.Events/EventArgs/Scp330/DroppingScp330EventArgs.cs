// -----------------------------------------------------------------------
// <copyright file="DroppingScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all information before a player drops a SCP-330 candy.
    /// </summary>
    public class DroppingScp330EventArgs : IPlayerEvent, IScp330Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="scp330">
        /// <inheritdoc cref="Scp330" />
        /// </param>
        /// <param name="candy">
        /// <inheritdoc cref="Candy" />
        /// </param>
        public DroppingScp330EventArgs(Player player, Scp330Bag scp330, CandyKindID candy)
        {
            Player = player;
            Scp330 = (Scp330)Item.Get(scp330);
            Candy = candy;
        }

        /// <inheritdoc/>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;

        /// <summary>
        /// Gets or sets a value indicating whether the type of candy drop.
        /// </summary>
        public CandyKindID Candy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }
    }
}