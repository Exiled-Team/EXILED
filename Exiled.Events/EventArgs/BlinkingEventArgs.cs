// -----------------------------------------------------------------------
// <copyright file="BlinkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

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
        /// <param name="triggers"><inheritdoc cref="Triggers"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        public BlinkingEventArgs(Player player, Player[] triggers, float duration)
        {
            Player = player;
            Triggers = triggers;
            Duration = duration;
        }

        /// <summary>
        /// Gets the SCP-173 player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the array of players, who triggered SCP-173.
        /// </summary>
        public Player[] Triggers { get; }

        /// <summary>
        /// Gets the blink duration.
        /// </summary>
        public float Duration { get; }
    }
}
