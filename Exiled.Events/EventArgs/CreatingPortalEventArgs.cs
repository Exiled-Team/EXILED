// -----------------------------------------------------------------------
// <copyright file="CreatingPortalEventArgs.cs" company="Exiled Team">
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
    /// Contains all informations before SCP-106 creates a portal.
    /// </summary>
    public class CreatingPortalEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatingPortalEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public CreatingPortalEventArgs(Player player, Vector3 position, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the portal position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can create a portal.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
