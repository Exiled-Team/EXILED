// -----------------------------------------------------------------------
// <copyright file="KickedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after kicking a player from the server.
    /// </summary>
    public class KickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KickedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public KickedEventArgs(Player player, string reason, bool isAllowed = true)
        {
            Player = player;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the kicked player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the kick reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
