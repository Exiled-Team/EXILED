// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before a player escapes the pocket dimension.
    /// </summary>
    public class EscapingPocketDimensionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingPocketDimensionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="teleportPosition"><inheritdoc cref="TeleportPosition"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EscapingPocketDimensionEventArgs(Player player, Vector3 teleportPosition, bool isAllowed = true)
        {
            Player = player;
            TeleportPosition = teleportPosition;
            IsAllowed = isAllowed;
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
        public bool IsAllowed { get; set; }
    }
}
