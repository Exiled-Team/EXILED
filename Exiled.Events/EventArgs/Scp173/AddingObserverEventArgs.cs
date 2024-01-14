// -----------------------------------------------------------------------
// <copyright file="AddingObserverEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player sees SCP-173.
    /// </summary>
    public class AddingObserverEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingObserverEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Scp173" /></param>
        /// <param name="target"><inheritdoc cref="Player" /></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed" /></param>
        public AddingObserverEventArgs(Player player, Player target, bool isAllowed = true)
        {
            Scp173 = player.Role.As<Scp173Role>();
            Player = player;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Scp173Role Scp173 { get; }

        /// <summary>
        /// Gets the target who has looked at SCP-173.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-173.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be added as an observer.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}