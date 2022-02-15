// -----------------------------------------------------------------------
// <copyright file="StoppingGeneratorEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all informations before a player ejects a tablet from a generator.
    /// </summary>
    public class StoppingGeneratorEventArgs : ActivatingGeneratorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoppingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's ejecting the tablet.</param>
        /// <param name="generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <param name="isAllowed">Indicates whether or not the tablet can be ejected.</param>
        public StoppingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
            : base(player, generator, isAllowed)
        {
        }
    }
}
