// -----------------------------------------------------------------------
// <copyright file="EatingScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Interfaces.Item;

    using InventorySystem.Items.Usables.Scp330;

    using Scp330 = Exiled.API.Features.Items.Scp330;

    /// <summary>
    ///     Contains all information before a player eats SCP-330.
    /// </summary>
    public class EatingScp330EventArgs : IPlayerEvent, IDeniableEvent, IItemScp330Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EatingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player" />.</param>
        /// <param name="scp330"><see cref="Scp330" />.</param>
        /// <param name="candy"><see cref="ICandy" />.</param>
        /// <param name="isAllowed"><see cref="IsAllowed" />.</param>
        public EatingScp330EventArgs(Player player, Scp330Bag scp330, ICandy candy, bool isAllowed = true)
        {
            Player = player;
            Scp330 = (Scp330)Item.Get(scp330);
            Candy = candy;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="ICandy" /> that is being eaten by the player.
        /// </summary>
        public ICandy Candy { get; }

        /// <summary>
        ///     Gets the Scp330 instance.
        /// </summary>
        public Scp330 Scp330 { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can eat SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's eating SCP-330.
        /// </summary>
        public Player Player { get; }
    }
}
