// -----------------------------------------------------------------------
// <copyright file="ClosingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
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
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="generator">
        ///     <inheritdoc cref="Generator" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ClosingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the generator can be closing.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the generator that is closing.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        ///     Gets the player who's closing the generator.
        /// </summary>
        public Player Player { get; }
    }
}
