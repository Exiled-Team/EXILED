// -----------------------------------------------------------------------
// <copyright file="SurfaceButtonInteractEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player interacts with the Warhead button in surface.
    /// </summary>
    public class SurfaceButtonInteractEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceButtonInteractEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public SurfaceButtonInteractEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's interacting with the Warhead button in surface.
        /// </summary>
        public Player Player { get; }
    }
}
