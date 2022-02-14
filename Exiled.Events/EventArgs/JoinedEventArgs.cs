// -----------------------------------------------------------------------
// <copyright file="JoinedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after a player joins the server.
    /// </summary>
    public class JoinedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public JoinedEventArgs(Player player)
        {
            Player = player;
        }

        /// <summary>
        /// Gets the joined player.
        /// </summary>
        public Player Player { get; }
    }
}
