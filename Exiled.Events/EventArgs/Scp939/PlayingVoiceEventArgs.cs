// -----------------------------------------------------------------------
// <copyright file="PlayingVoiceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 plays a stolen player's voice.
    /// </summary>
    public class PlayingVoiceEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayingVoiceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="stolen">
        ///     The player who's voice was stolen.
        /// </param>
        public PlayingVoiceEventArgs(ReferenceHub player, ReferenceHub stolen)
        {
            Player = Player.Get(player);
            Target = Player.Get(stolen);
        }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;

        /// <inheritdoc />
        public Player Target { get; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}