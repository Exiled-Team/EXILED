// -----------------------------------------------------------------------
// <copyright file="BlinkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a players blink near SCP-173.
    /// </summary>
    public class BlinkingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="targets"><inheritdoc cref="Targets"/></param>
        public BlinkingEventArgs(Player player, List<Player> targets)
        {
            Player = player;
            Targets = targets;
        }

        /// <summary>
        /// Gets the player who controlling SCP-173.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a list of players who have triggered SCP-173.
        /// </summary>
        public List<Player> Targets { get; }
    }
}
