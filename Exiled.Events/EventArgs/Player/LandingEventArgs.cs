// -----------------------------------------------------------------------
// <copyright file="LandingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all the information after a <see cref="API.Features.Player" /> lands on the ground.
    /// </summary>
    public class LandingEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LandingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public LandingEventArgs(Player player) => Player = player;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> who's landing.
        /// </summary>
        public Player Player { get; }
    }
}