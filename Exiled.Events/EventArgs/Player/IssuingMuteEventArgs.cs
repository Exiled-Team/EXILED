// -----------------------------------------------------------------------
// <copyright file="IssuingMuteEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before muting a player.
    /// </summary>
    public class IssuingMuteEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssuingMuteEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isIntercom">
        /// <inheritdoc cref="IsIntercom" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public IssuingMuteEventArgs(Player player, bool isIntercom, bool isAllowed = true)
        {
            Player = player;
            IsIntercom = isIntercom;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's being muted.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is being intercom muted or not.
        /// </summary>
        public bool IsIntercom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be muted.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}