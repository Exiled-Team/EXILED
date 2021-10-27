// -----------------------------------------------------------------------
// <copyright file="EatingSCP330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after a player eats a SCP330.
    /// </summary>
    public class EatingSCP330EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EatingSCP330EventArgs"/> class.
        /// Test.
        /// </summary>
        /// <param name="player"><see cref="Player"/>.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public EatingSCP330EventArgs(Player player, bool isAllowed = false)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's eated SCP330.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not player is allowed to eat SCP330.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
