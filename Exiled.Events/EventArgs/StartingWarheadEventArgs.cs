// -----------------------------------------------------------------------
// <copyright file="StartingWarheadEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before starting the warhead.
    /// </summary>
    public class StartingWarheadEventArgs : StoppingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingWarheadEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's going to start the warhead.</param>
        /// <param name="isAllowed">Indicating whether the event can be executed or not.</param>
        public StartingWarheadEventArgs(Player player, bool isAllowed = true)
            : base(player, isAllowed)
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not the nuke was set off automatically.
        /// </summary>
        public bool IsAuto { get; } = AlphaWarheadController.Host._autoDetonate && AlphaWarheadController.Host._autoDetonateTime <= 0;
    }
}
