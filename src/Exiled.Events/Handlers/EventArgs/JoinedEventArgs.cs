// -----------------------------------------------------------------------
// <copyright file="JoinedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all player's informations, after he joins the server.
    /// </summary>
    public class JoinedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinedEventArgs"/> class.
        /// </summary>
        /// <param name="player">The joined player.</param>
        public JoinedEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the joined player.
        /// </summary>
        public Player Player { get; private set; }
    }
}
