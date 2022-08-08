// -----------------------------------------------------------------------
// <copyright file="KillingByCollisionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player is killed by collision.
    /// </summary>
    public class KillingByCollisionEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KillingByCollisionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public KillingByCollisionEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's currently being killing by collision.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is killed by collision.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
