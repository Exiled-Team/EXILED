// -----------------------------------------------------------------------
// <copyright file="MakingNoiseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player makes noise.
    /// </summary>
    public class MakingNoiseEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MakingNoiseEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="volume">
        ///     <inheritdoc cref="Volume" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public MakingNoiseEventArgs(Player player, float volume, bool isAllowed = true)
        {
            Player = player;
            Volume = volume;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the volume.
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can make noise.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's making noise.
        /// </summary>
        public Player Player { get; }
    }
}