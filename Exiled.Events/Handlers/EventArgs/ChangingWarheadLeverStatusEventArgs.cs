// -----------------------------------------------------------------------
// <copyright file="ChangingWarheadLeverStatusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player changes the warhead lever.
    /// </summary>
    public class ChangingWarheadLeverStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingWarheadLeverStatusEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingWarheadLeverStatusEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's changing the warhead status.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}