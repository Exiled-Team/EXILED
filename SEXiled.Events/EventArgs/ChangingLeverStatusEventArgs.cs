// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatusEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before a player changes the warhead lever status.
    /// </summary>
    public class ChangingLeverStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingLeverStatusEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="curState"><inheritdoc cref="CurrentState"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingLeverStatusEventArgs(Player player, bool curState, bool isAllowed = true)
        {
            Player = player;
            CurrentState = curState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's changing the warhead status.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether the lever is enabled.
        /// </summary>
        public bool CurrentState { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the lever status will change.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
