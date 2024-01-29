// -----------------------------------------------------------------------
// <copyright file="DestroyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player's object is destroyed.
    /// </summary>
    public class DestroyingEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DestroyingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public DestroyingEventArgs(Player player)
        {
            Player = player;
#if DEBUG
            Log.Debug($"Destroying obj for {player.Nickname}");
#endif
        }

        /// <summary>
        ///     Gets the destroying player.
        /// </summary>
        public Player Player { get; }
    }
}
