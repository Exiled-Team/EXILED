// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a spectator changes the spectated player.
    /// </summary>
    public class ChangingSpectatedPlayerEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingSpectatedPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldTarget"><inheritdoc cref="OldTarget"/></param>
        /// <param name="newTarget"><inheritdoc cref="NewTarget"/></param>
        public ChangingSpectatedPlayerEventArgs(Player player, Player oldTarget, Player newTarget)
        {
            Player = player;
            OldTarget = oldTarget;
            NewTarget = newTarget;
        }

        /// <summary>
        /// Gets player that is changing spectated player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets player that was spectated.
        /// </summary>
        public Player OldTarget { get; }

        /// <summary>
        /// Gets the player who's going to be spectated.
        /// </summary>
        public Player NewTarget { get; }
    }
}
