// -----------------------------------------------------------------------
// <copyright file="ActivatingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all information before a player filps the switch to a generator.
    /// </summary>
    public class ActivatingGeneratorEventArgs : IPlayerEvent, IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingGeneratorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="generator">
        /// <inheritdoc cref="Generator" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ActivatingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the switch can be flipped.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="Generator" /> instance.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        /// Gets the player who's filpping the switch to the generator.
        /// </summary>
        public Player Player { get; }
    }
}