// -----------------------------------------------------------------------
// <copyright file="OpeningGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player opens a generator.
    /// </summary>
    public class OpeningGeneratorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public OpeningGeneratorEventArgs(Player player, Generator079 generator, bool isAllowed = true)
        {
            Player = player;
            Generator = generator;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's opening the generator.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the generator that is opening.
        /// </summary>
        public Generator079 Generator { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}