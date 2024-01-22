// -----------------------------------------------------------------------
// <copyright file="ShowingHitMarkerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before hit marker is showing.
    /// </summary>
    public class ShowingHitMarkerEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowingHitMarkerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="size"><inheritdoc cref="Size"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ShowingHitMarkerEventArgs(Player player, float size, bool isAllowed = true)
        {
            Player = player;
            Size = size;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a hit marker size.
        /// </summary>
        public float Size { get; set; }
    }
}