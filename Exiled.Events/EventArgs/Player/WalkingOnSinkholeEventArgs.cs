// -----------------------------------------------------------------------
// <copyright file="WalkingOnSinkholeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player walks over a sinkhole.
    /// </summary>
    [Obsolete("Use StayingOnEnvironmentalHazardEventArgs event instead.", true)]
    public class WalkingOnSinkholeEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WalkingOnSinkholeEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="sinkhole">
        ///     <inheritdoc cref="SinkholeEnvironmentalHazard" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public WalkingOnSinkholeEventArgs(Player player, SinkholeEnvironmentalHazard sinkhole, bool isAllowed = true)
        {
            Player = player;
            Sinkhole = sinkhole;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the sinkhole that the player is walking on.
        /// </summary>
        public SinkholeEnvironmentalHazard Sinkhole { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player's data can be synced with the server.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player walking on the sinkhole.
        /// </summary>
        public Player Player { get; }
    }
}