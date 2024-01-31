// -----------------------------------------------------------------------
// <copyright file="BannedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information after banning a player from the server.
    /// </summary>
    public class BannedEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BannedEventArgs" /> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target" /></param>
        /// <param name="issuer"><inheritdoc cref="Player" /></param>
        /// <param name="details"><inheritdoc cref="Details" /></param>
        /// <param name="type"><inheritdoc cref="Type" /></param>
        /// <param name="isForced"><inheritdoc cref="IsForced" /></param>
        public BannedEventArgs(Player target, Player issuer, BanDetails details, BanHandler.BanType type, bool isForced)
        {
            Target = target;
            Player = issuer;
            Details = details;
            Type = type;
            IsForced = isForced;
        }

        /// <summary>
        /// Gets the banned player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the issuer player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the ban details.
        /// </summary>
        public BanDetails Details { get; }

        /// <summary>
        /// Gets the ban type.
        /// </summary>
        public BanHandler.BanType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the ban is forced or not.
        /// </summary>
        public bool IsForced { get; }
    }
}