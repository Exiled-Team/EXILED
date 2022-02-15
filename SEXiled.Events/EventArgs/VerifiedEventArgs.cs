// -----------------------------------------------------------------------
// <copyright file="VerifiedEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations after the server verifies a player.
    /// </summary>
    public class VerifiedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public VerifiedEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the verified player.
        /// </summary>
        public Player Player { get; }
    }
}
