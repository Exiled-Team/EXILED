// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player escapes the pocket dimension.
    /// </summary>
    public class EscapingPocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingPocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        /// <inheritdoc cref="TeleportPosition" />
        /// </param>
        public EscapingPocketDimensionEventArgs(Player player, Vector3 position)
        {
            Player = player;
            TeleportPosition = position;
        }

        /// <summary>
        /// Gets the player who's escaping the pocket dimension.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the position in which the player is going to be teleported to.
        /// </summary>
        public Vector3 TeleportPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can successfully escape the pocket dimension.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}