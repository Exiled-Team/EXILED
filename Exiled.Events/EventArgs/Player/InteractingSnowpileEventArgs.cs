// -----------------------------------------------------------------------
// <copyright file="InteractingSnowpileEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a <see cref="API.Features.Player"/> is interacting with snowpile.
    /// </summary>
    public class InteractingSnowpileEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingSnowpileEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="snowpile"><inheritdoc cref="Snowpile"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingSnowpileEventArgs(Player player, Snowpile snowpile, bool isAllowed = true)
        {
            Player = player;
            Snowpile = snowpile;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a snowpile with which player is interacting.
        /// </summary>
        public Snowpile Snowpile { get; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not should be checked if player already has a snowball.
        /// </summary>
        public bool ShouldCheck { get; set; } = true;
    }
}