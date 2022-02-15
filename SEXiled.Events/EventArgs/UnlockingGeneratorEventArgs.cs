// -----------------------------------------------------------------------
// <copyright file="UnlockingGeneratorEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all informations before a generator is unlocked.
    /// </summary>
    public class UnlockingGeneratorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnlockingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's unlocking the generator.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the generator that is going to be unlocked.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the generator can be unlocked.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
