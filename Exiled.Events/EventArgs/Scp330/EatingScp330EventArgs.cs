// -----------------------------------------------------------------------
// <copyright file="EatingScp330EventArgs.cs" company="Exiled Team">
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
    /// Contains all information before a player eats SCP-330.
    /// </summary>
    public class EatingScp330EventArgs : IPlayerEvent, IScp330Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player"><see cref="Player" />.</param>
        /// <param name="candy"><see cref="ICandy" />.</param>
        /// <param name="isAllowed"><see cref="IsAllowed" />.</param>
        public EatingScp330EventArgs(Player player, ICandy candy, bool isAllowed = true)
        {
            Player = player;
            Candy = candy;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the <see cref="ICandy" /> that is being eaten by the player.
        /// </summary>
        public ICandy Candy { get; set; }

        /// <inheritdoc/>
        public Scp330 Scp330 { get; }

        /// <inheritdoc/>
        public Item Item => Scp330;

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}