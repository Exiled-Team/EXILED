// -----------------------------------------------------------------------
// <copyright file="MakingNoiseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player makes noise.
    /// </summary>
    public class MakingNoiseEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MakingNoiseEventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="distance"><inheritdoc cref="Distance"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public MakingNoiseEventArgs(Player player, float distance, bool isAllowed = true)
        {
            Player = player;
            Distance = distance;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's making noise.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the footsteps distance.
        /// </summary>
        public float Distance { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}