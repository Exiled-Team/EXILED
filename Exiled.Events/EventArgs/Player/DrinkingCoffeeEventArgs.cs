// -----------------------------------------------------------------------
// <copyright file="DrinkingCoffeeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before player interacts with coffee cup.
    /// </summary>
    public class DrinkingCoffeeEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrinkingCoffeeEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="coffee"><inheritdoc cref="Coffee"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DrinkingCoffeeEventArgs(Player player, Coffee coffee, bool isAllowed = true)
        {
            Player = player;
            Coffee = coffee;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the coffee with which player is interacting.
        /// </summary>
        public Coffee Coffee { get; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}