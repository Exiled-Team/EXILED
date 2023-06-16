// -----------------------------------------------------------------------
// <copyright file="ChangingIntercomMuteStatusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player's intercom mute status is changed.
    /// </summary>
    public class ChangingIntercomMuteStatusEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingIntercomMuteStatusEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isMuted">
        ///     <inheritdoc cref="IsMuted" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingIntercomMuteStatusEventArgs(Player player, bool isMuted, bool isAllowed = true)
        {
            Player = player;
            IsMuted = isMuted;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        ///     Gets a value indicating whether the player is being intercom muted or unmuted.
        /// </summary>
        public bool IsMuted { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}