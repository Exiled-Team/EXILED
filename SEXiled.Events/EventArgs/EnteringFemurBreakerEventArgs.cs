// -----------------------------------------------------------------------
// <copyright file="EnteringFemurBreakerEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before a player enters the femur breaker.
    /// </summary>
    public class EnteringFemurBreakerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnteringFemurBreakerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EnteringFemurBreakerEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's entering the femur breaker.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can activate the femur breaker.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
