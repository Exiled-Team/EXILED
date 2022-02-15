// -----------------------------------------------------------------------
// <copyright file="EatenScp330EventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all informations after a player has eaten SCP-330.
    /// </summary>
    public class EatenScp330EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatenScp330EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/>.</param>
        /// <param name="candy"><inheritdoc cref="Candy"/>.</param>
        public EatenScp330EventArgs(Player player, ICandy candy)
        {
            Player = player;
            Candy = candy;
        }

        /// <summary>
        /// Gets the player who's eaten SCP-330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="ICandy"/> that was eaten by the player.
        /// </summary>
        public ICandy Candy { get; }
    }
}
