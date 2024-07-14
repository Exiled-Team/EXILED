// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player changes the warhead lever status.
    /// </summary>
    public class ChangingLeverStatusEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingLeverStatusEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="curState">
        /// <inheritdoc cref="CurrentState" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingLeverStatusEventArgs(Player player, bool curState, bool isAllowed = true)
        {
            Player = player;
            CurrentState = curState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating whether the lever is enabled.
        /// </summary>
        public bool CurrentState { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the lever status will change.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's changing the warhead status.
        /// </summary>
        public Player Player { get; }
    }
}