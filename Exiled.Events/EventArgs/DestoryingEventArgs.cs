// -----------------------------------------------------------------------
// <copyright file="DestoryingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player's object is destroyed.
    /// </summary>
    public class DestoryingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestoryingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public DestoryingEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the destoying player.
        /// </summary>
        public Player Player { get; }
    }
}
