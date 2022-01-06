// -----------------------------------------------------------------------
// <copyright file="LandingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all the information when a player lands.
    /// </summary>
    public class LandingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LandingEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player landing.</param>
        public LandingEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the player landing.
        /// </summary>
        public Player Player { get; }
    }
}
