// -----------------------------------------------------------------------
// <copyright file="ClosingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player closes a generator.
    /// </summary>
    public class ClosingGeneratorEventArgs : OpeningGeneratorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's closing the generator.</param>
        /// <param name="generator">The <see cref="Generator079"/> instance.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public ClosingGeneratorEventArgs(Player player, Generator079 generator, bool isAllowed = true)
            : base(player, generator, isAllowed)
        {
        }
    }
}