// -----------------------------------------------------------------------
// <copyright file="ClosingGeneratorEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before a player closes a generator.
    /// </summary>
    public class ClosingGeneratorEventArgs : IPlayerEvent, IDeniableEvent, IGeneratorEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClosingGeneratorEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's closing the generator.</param>
        /// <param name="generator">The <see cref="Scp079Generator" /> instance.</param>
        /// <param name="isAllowed">Indicates whether or not the generator can be closed.</param>
        public ClosingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the generator can be opened.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the generator that is opening.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        ///     Gets the player who's opening the generator.
        /// </summary>
        public Player Player { get; }
    }
}