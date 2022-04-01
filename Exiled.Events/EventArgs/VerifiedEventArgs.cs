// -----------------------------------------------------------------------
// <copyright file="VerifiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after the server verifies a player.
    /// </summary>
    public class VerifiedEventArgs : EventArgs {
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
