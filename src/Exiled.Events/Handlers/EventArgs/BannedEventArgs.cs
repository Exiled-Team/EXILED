// -----------------------------------------------------------------------
// <copyright file="BannedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations after banning a player from the server.
    /// </summary>
    public class BannedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BannedEventArgs"/> class.
        /// </summary>
        /// <param name="player">The banned player.</param>
        /// <param name="details">The ban details.</param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        public BannedEventArgs(Player player, BanDetails details, BanHandler.BanType type)
        {
            Player = player;
            Details = details;
            Type = type;
        }

        /// <summary>
        /// Gets the banned player.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the ban details.
        /// </summary>
        public BanDetails Details { get; private set; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType Type { get; private set; }
    }
}