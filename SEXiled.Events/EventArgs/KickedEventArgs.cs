// -----------------------------------------------------------------------
// <copyright file="KickedEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations after kicking a player from the server.
    /// </summary>
    public class KickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KickedEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        public KickedEventArgs(Player target, string reason)
        {
            Target = target;
            Reason = reason;
        }

        /// <summary>
        /// Gets the kicked player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the kick reason.
        /// </summary>
        public string Reason { get; }
    }
}
