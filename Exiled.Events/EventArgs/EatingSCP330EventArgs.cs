// -----------------------------------------------------------------------
// <copyright file="EatingSCP330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// Contains all informations before a player eats SCP330.
    /// </summary>
    public class EatingSCP330EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatingSCP330EventArgs"/> class.
        /// </summary>
        /// <param name="player"><see cref="Player"/>.</param>
        /// <param name="candy"><see cref="ICandy"/>.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public EatingSCP330EventArgs(Player player, ICandy candy, bool isAllowed = true)
        {
            Player = player;
            Candy = candy;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's eating SCP330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="ICandy"/> that the player is eating.
        /// </summary>
        public ICandy Candy { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not player is allowed to eat SCP330.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
