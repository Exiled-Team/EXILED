// -----------------------------------------------------------------------
// <copyright file="UnlockingGeneratorEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before a generator is unlocked.
    /// </summary>
    public class UnlockingGeneratorEventArgs : IPlayerEvent, IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnlockingGeneratorEventArgs" /> class.
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
        public UnlockingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
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