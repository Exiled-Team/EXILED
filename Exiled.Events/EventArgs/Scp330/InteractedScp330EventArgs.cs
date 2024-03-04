// -----------------------------------------------------------------------
// <copyright file="InteractedScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all information after <see cref="API.Features.Player"/> interacts with SCP-330.
    /// </summary>
    public class InteractedScp330EventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractedScp330EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp330Bag"><inheritdoc cref="Scp330"/></param>
        /// <param name="candyKindID"><inheritdoc cref="CandyKindID"/></param>
        public InteractedScp330EventArgs(Player player, Scp330Bag scp330Bag, CandyKindID candyKindID)
        {
            Player = player;
            Scp330 = Item.Get(scp330Bag).As<Scp330>();
            CandyKindID = candyKindID;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the bag in which candy was added.
        /// </summary>
        public Scp330 Scp330 { get; }

        /// <summary>
        /// Gets the added candy.
        /// </summary>
        public CandyKindID CandyKindID { get; }
    }
}