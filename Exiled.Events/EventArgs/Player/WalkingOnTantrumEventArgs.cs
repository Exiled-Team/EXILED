// -----------------------------------------------------------------------
// <copyright file="WalkingOnTantrumEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before a player walks over a tantrum.
    /// </summary>
    [Obsolete("Use StayingOnEnvironmentalHazardEventArgs event instead.", true)]
    public class WalkingOnTantrumEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WalkingOnTantrumEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="tantrum">
        ///     <inheritdoc cref="Tantrum" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public WalkingOnTantrumEventArgs(Player player, TantrumEnvironmentalHazard tantrum, bool isAllowed = true)
        {
            Player = player;
            Tantrum = tantrum;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the tantrum that the player is walking on.
        /// </summary>
        public TantrumEnvironmentalHazard Tantrum { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player's data can be synced with the server.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's walking on the tantrum.
        /// </summary>
        public Player Player { get; }
    }
}