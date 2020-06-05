// -----------------------------------------------------------------------
// <copyright file="CreatingScp106PortalEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Contains all informations before creating a portal with SCP-096.
    /// </summary>
    public class CreatingScp106PortalEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatingScp106PortalEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public CreatingScp106PortalEventArgs(Player player, Vector3 position, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-096.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets the portal position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}