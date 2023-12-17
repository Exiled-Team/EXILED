// -----------------------------------------------------------------------
// <copyright file="StoppingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all information before a player turns off a generator.
    /// </summary>
    public class StoppingGeneratorEventArgs : IPlayerEvent, IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoppingGeneratorEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's flipping the switch.</param>
        /// <param name="generator">The <see cref="Scp079Generator" /> instance.</param>
        /// <param name="isAllowed">Indicates whether or not the switch of the generator can be flipped.</param>
        public StoppingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the switch can be flipped.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="Generator" /> instance.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        /// Gets the player who's filpping the switch of the generator.
        /// </summary>
        public Player Player { get; }
    }
}