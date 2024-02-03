// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a spectator changes the spectated player.
    /// </summary>
    public class ChangingSpectatedPlayerEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingSpectatedPlayerEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldTarget">
        /// <inheritdoc cref="OldTarget" />
        /// </param>
        /// <param name="newTarget">
        /// <inheritdoc cref="NewTarget" />
        /// </param>
        public ChangingSpectatedPlayerEventArgs(ReferenceHub player, uint oldTarget, uint newTarget)
        {
            Player = Player.Get(player);
            OldTarget = Player.Get(oldTarget);
            NewTarget = Player.Get(newTarget);
        }

        /// <summary>
        /// Gets player that was being spectated.
        /// </summary>
        public Player OldTarget { get; }

        /// <summary>
        /// Gets the player who's going to be spectated.
        /// </summary>
        public Player NewTarget { get; }

        /// <summary>
        /// Gets player that is changing spectated player.
        /// </summary>
        public Player Player { get; }
    }
}