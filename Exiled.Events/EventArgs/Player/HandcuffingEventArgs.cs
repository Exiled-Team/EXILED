// -----------------------------------------------------------------------
// <copyright file="HandcuffingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before handcuffing a player.
    /// </summary>
    public class HandcuffingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandcuffingEventArgs" /> class.
        /// </summary>
        /// <param name="cuffer">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public HandcuffingEventArgs(Player cuffer, Player target, bool isAllowed = true)
        {
            Player = cuffer;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who is getting cuffed.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can be handcuffed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the cuffer player.
        /// </summary>
        public Player Player { get; }
    }
}