// -----------------------------------------------------------------------
// <copyright file="DroppingScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all informations before a player drop a candy of SCP-330.
    /// </summary>
    public class DroppingScp330EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingScp330EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp330"><inheritdoc cref="Item"/></param>
        /// <param name="candy"><inheritdoc cref="Candy"/></param>
        public DroppingScp330EventArgs(Player player, Scp330Bag scp330, CandyKindID candy)
        {
            Player = player;
            Item = Item.Get(scp330);
            Candy = candy;
        }

        /// <summary>
        /// Gets the player who's interacting with SCP-330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value representing the <see cref="Item"/> being picked up.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the type of candy drop.
        /// </summary>
        public CandyKindID Candy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
