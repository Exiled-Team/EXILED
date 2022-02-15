// -----------------------------------------------------------------------
// <copyright file="WalkingOnTantrumEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all information before a player walks over a tantrum.
    /// </summary>
    public class WalkingOnTantrumEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkingOnTantrumEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="tantrum"><inheritdoc cref="Tantrum"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public WalkingOnTantrumEventArgs(Player player, TantrumEnvironmentalHazard tantrum, bool isAllowed = true)
        {
            Player = player;
            Tantrum = tantrum;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's walking on the tantrum.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the tantrum that the player is walking on.
        /// </summary>
        public TantrumEnvironmentalHazard Tantrum { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's data can be synced with the server.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
