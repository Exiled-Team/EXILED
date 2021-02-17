// -----------------------------------------------------------------------
// <copyright file="TeleportingEventArgs.cs" company="Exiled Team">
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
    /// Contains all informations before SCP-106 teleports using a portal.
    /// </summary>
    public class TeleportingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="portalPosition"><inheritdoc cref="PortalPosition"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TeleportingEventArgs(Player player, Vector3 portalPosition, bool isAllowed = true)
        {
            Player = player;
            PortalPosition = portalPosition;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the portal position.
        /// </summary>
        public Vector3 PortalPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can teleport using a portal.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
