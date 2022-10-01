// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before a player escapes the pocket dimension.
    /// </summary>
    public class EscapingPocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EscapingPocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="TeleportPosition" />
        /// </param>
        /// <param name="num1">Distance number 1.</param>
        /// <param name="num2">Distance number 2.</param>
        public EscapingPocketDimensionEventArgs(Player player, SafeTeleportPosition position, float num1, float num2)
        {
            Player = player;
            TeleportPosition = num1 < num2 ? position.SafePositions[0].position : position.SafePositions[1].position;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EscapingPocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="TeleportPosition" />
        /// </param>
        public EscapingPocketDimensionEventArgs(Player player, Vector3 position)
        {
            Player = player;
            TeleportPosition = position;
        }

        /// <summary>
        ///     Gets or sets the position in which the player is going to be teleported to.
        /// </summary>
        public Vector3 TeleportPosition { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can successfully escape the pocket dimension.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who's escaping the pocket dimension.
        /// </summary>
        public Player Player { get; }
    }
}