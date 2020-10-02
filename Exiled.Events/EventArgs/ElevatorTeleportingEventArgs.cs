// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player teleports to another zone.
    /// </summary>
    public class ElevatorTeleportingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorTeleportEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="camera"><inheritdoc cref="Camera079"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ElevatorTeleportingEventArgs(Player player, Camera079 camera, bool isAllowed = true)
        {
            Player = player;
            Camera = camera;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="Camera079"/> instance.
        /// </summary>
        public Camera079 Camera { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
