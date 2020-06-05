// -----------------------------------------------------------------------
// <copyright file="StartingWarheadEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before the SCP-914 machine upgrades items inside it.
    /// </summary>
    public class StartingWarheadEventArgs : StoppingWarheadEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingWarheadEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's going to stop the warhead.</param>
        /// <param name="isAllowed">Indicating whether the event can be executed or not.</param>
        public StartingWarheadEventArgs(Player player, bool isAllowed = true)
            : base(player, isAllowed)
        {
        }
    }
}