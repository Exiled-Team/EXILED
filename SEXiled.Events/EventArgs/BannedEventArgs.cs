// -----------------------------------------------------------------------
// <copyright file="BannedEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations after banning a player from the server.
    /// </summary>
    public class BannedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BannedEventArgs"/> class.
        /// </summary>
        /// <param name="target">The banned player.</param>
        /// <param name="issuer">The issuer player.</param>
        /// <param name="details">The ban details.</param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        public BannedEventArgs(Player target, Player issuer, BanDetails details, BanHandler.BanType type)
        {
            Target = target;
            Details = details;
            Type = type;
            Issuer = issuer;
        }

        /// <summary>
        /// Gets the banned player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the issuer player.
        /// </summary>
        public Player Issuer { get; }

        /// <summary>
        /// Gets the ban details.
        /// </summary>
        public BanDetails Details { get; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType Type { get; }
    }
}
