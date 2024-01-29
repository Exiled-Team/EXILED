// -----------------------------------------------------------------------
// <copyright file="StartingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before starting the warhead.
    /// </summary>
    public class StartingEventArgs : StoppingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's going to start the warhead.</param>
        /// <param name="isAuto">Indicating whether or not the nuke was set off automatically.</param>
        /// <param name="isAllowed">Indicating whether the event can be executed or not.</param>
        public StartingEventArgs(Player player, bool isAuto, bool isAllowed = true)
            : base(player, isAllowed)
        {
            IsAuto = isAuto;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the nuke was set off automatically.
        /// </summary>
        public bool IsAuto { get; set; }
    }
}