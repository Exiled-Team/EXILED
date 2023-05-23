// -----------------------------------------------------------------------
// <copyright file="KillingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PlayerStatsSystem;

namespace Exiled.Events.EventArgs.Player
{
    using Interfaces;

    /// <summary>
    /// Contains all information before player data to kill player is sent.
    /// </summary>
    public class KillingPlayerEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KillingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="handler"><inheritdoc cref="Handler"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public KillingPlayerEventArgs(API.Features.Player player, DamageHandlerBase handler, bool isAllowed = true)
        {
            Player = player;
            Handler = handler;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets current player.
        /// </summary>
        public API.Features.Player Player { get; set; }

        /// <summary>
        /// Gets or sets current Damage Handler.
        /// </summary>
        public DamageHandlerBase Handler { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}