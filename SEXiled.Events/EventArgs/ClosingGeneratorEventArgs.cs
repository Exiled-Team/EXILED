// -----------------------------------------------------------------------
// <copyright file="ClosingGeneratorEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all informations before a player closes a generator.
    /// </summary>
    public class ClosingGeneratorEventArgs : OpeningGeneratorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's closing the generator.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <param name="isAllowed">Indicates whether or not the generator can be closed.</param>
        public ClosingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
            : base(player, generator, isAllowed)
        {
        }
    }
}
