// -----------------------------------------------------------------------
// <copyright file="StartingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before starting the warhead.
    /// </summary>
    public class StartingEventArgs : StoppingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's going to start the warhead.</param>
        /// <param name="isAllowed">Indicating whether the event can be executed or not.</param>
        public StartingEventArgs(Player player, bool isAllowed = true)
            : base(player, isAllowed)
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not the nuke was set off automatically.
        /// </summary>
        public bool IsAuto { get; } = AlphaWarheadController.Host._autoDetonate && AlphaWarheadController.Host._autoDetonateTime <= 0;
    }
}
