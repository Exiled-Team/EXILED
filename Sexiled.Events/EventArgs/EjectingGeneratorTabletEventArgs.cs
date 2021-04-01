// -----------------------------------------------------------------------
// <copyright file="EjectingGeneratorTabletEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;

namespace Sexiled.Events.EventArgs
{
    using Sexiled.API.Features;

    /// <summary>
    /// Contains all informations before a player ejects a tablet from a generator.
    /// </summary>
    public class EjectingGeneratorTabletEventArgs : InsertingGeneratorTabletEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EjectingGeneratorTabletEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's ejecting the tablet.</param>
        /// <param name="generator">The <see cref="Generator079"/> instance.</param>
        /// <param name="isAllowed">Indicates whether or not the tablet can be ejected.</param>
        public EjectingGeneratorTabletEventArgs(Player player, Generator079 generator, bool isAllowed = true)
            : base(player, generator, isAllowed)
        {
        }
    }
}
