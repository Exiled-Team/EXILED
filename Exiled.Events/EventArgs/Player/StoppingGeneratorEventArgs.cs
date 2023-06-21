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
    ///     Contains all information before a player flips the switch of the generator.
    /// </summary>
    public class StoppingGeneratorEventArgs : IPlayerEvent, IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StoppingGeneratorEventArgs" /> class.
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

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Generator Generator { get; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}