// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleportingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 changes rooms via elevator.
    /// </summary>
    public class ElevatorTeleportingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorTeleportingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="camera"><inheritdoc cref="Camera"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ElevatorTeleportingEventArgs(Player player, Camera079 camera, float auxiliaryPowerCost, bool isAllowed = true)
        {
            Player = player;
            Camera = Camera.Get(camera);
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who is controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.Camera"/> that SCP-079 will be moved to.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to teleport to an elevator camera.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can teleport.
        /// Defaults to a <see cref="bool"/> describing whether or not SCP-079 has enough auxiliary power to teleport.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
