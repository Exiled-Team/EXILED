// -----------------------------------------------------------------------
// <copyright file="ActivatingScp914EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player activates SCP-914.
    /// </summary>
    public class ActivatingScp914EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingScp914EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ActivatingScp914EventArgs(Player player, double duration, bool isAllowed = true)
        {
            Player = player;
            Duration = duration;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's activating SCP-914.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets for how long SCP-914 would be occupied.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}