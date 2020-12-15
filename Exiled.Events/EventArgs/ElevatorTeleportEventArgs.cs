// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleportEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 changes rooms via elevator.
    /// </summary>
    public class ElevatorTeleportEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorTeleportEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="camera"><inheritdoc cref="Camera"/></param>
        /// <param name="apCost"><inheritdoc cref="APCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ElevatorTeleportEventArgs(Player player, Camera079 camera, float apCost, bool isAllowed = true)
        {
            Player = player;
            Camera = camera;
            APCost = apCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who is controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="Camera079"/> that SCP-079 will be moved to.
        /// </summary>
        public Camera079 Camera { get; set; }

        /// <summary>
        /// Gets or sets the amount of AP will be consumed during the level change.
        /// </summary>
        public float APCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can teleport.
        /// Defaults to a <see cref="bool"/> describing whether or not SCP-079 has enough AP to teleport.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
