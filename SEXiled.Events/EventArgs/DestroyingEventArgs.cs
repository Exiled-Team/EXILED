// -----------------------------------------------------------------------
// <copyright file="DestroyingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before a player's object is destroyed.
    /// </summary>
    public class DestroyingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestroyingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public DestroyingEventArgs(Player player)
        {
            Player = player;
#if DEBUG
            Log.Debug($"Destroying obj for {player.Nickname}");
#endif
        }

        /// <summary>
        /// Gets the destoying player.
        /// </summary>
        public Player Player { get; }
    }
}
