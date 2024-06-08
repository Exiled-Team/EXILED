// -----------------------------------------------------------------------
// <copyright file="EatenScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all information after a player has eaten SCP-330.
    /// </summary>
    public class EatenScp330EventArgs : IPlayerEvent, IScp330Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatenScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" />.</param>
        /// <param name="scp330"><inheritdoc cref="Scp330" />.</param>
        /// <param name="candy"><inheritdoc cref="Candy" />.</param>
        public EatenScp330EventArgs(Player player, Scp330Bag scp330, ICandy candy)
        {
            Player = player;
            Scp330 = Item.Get(scp330).As<Scp330>();
            Candy = candy;
        }

        /// <summary>
        /// Gets the <see cref="ICandy" /> that was eaten by the player.
        /// </summary>
        public ICandy Candy { get; }

        /// <inheritdoc/>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;

        /// <inheritdoc/>
        public Player Player { get; }
    }
}