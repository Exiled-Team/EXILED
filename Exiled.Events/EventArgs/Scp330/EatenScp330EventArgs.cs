// -----------------------------------------------------------------------
// <copyright file="EatenScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;

    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    ///     Contains all information after a player has eaten SCP-330.
    /// </summary>
    public class EatenScp330EventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EatenScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" />.</param>
        /// <param name="candy"><inheritdoc cref="Candy" />.</param>
        public EatenScp330EventArgs(Player player, ICandy candy)
        {
            Player = player;
            Candy = candy;
        }

        /// <summary>
        ///     Gets the <see cref="ICandy" /> that was eaten by the player.
        /// </summary>
        public ICandy Candy { get; }

        /// <summary>
        ///     Gets the player who has eaten SCP-330.
        /// </summary>
        public Player Player { get; }
    }
}