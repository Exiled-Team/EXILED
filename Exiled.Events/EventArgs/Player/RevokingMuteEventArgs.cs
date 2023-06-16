// -----------------------------------------------------------------------
// <copyright file="RevokingMuteEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before unmuting a player.
    /// </summary>
    public class RevokingMuteEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RevokingMuteEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isIntercom">
        ///     <inheritdoc cref="IsIntercom" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public RevokingMuteEventArgs(Player player, bool isIntercom, bool isAllowed = true)
        {
            Player = player;
            IsIntercom = isIntercom;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the player is being intercom muted or not.
        /// </summary>
        public bool IsIntercom { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}