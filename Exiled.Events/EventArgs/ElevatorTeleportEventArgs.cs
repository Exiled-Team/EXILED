// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleportEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before SCP-079 changes rooms via elevator.
    /// </summary>
    public class ElevatorTeleportEventArgs : ElevatorTeleportingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorTeleportEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="ElevatorTeleportingEventArgs.Player"/></param>
        /// <param name="camera"><inheritdoc cref="ElevatorTeleportingEventArgs.Camera"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="ElevatorTeleportingEventArgs.AuxiliaryPowerCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ElevatorTeleportingEventArgs.IsAllowed"/></param>
        public ElevatorTeleportEventArgs(Player player, Camera079 camera, float auxiliaryPowerCost, bool isAllowed = true)
            : base(player, camera, auxiliaryPowerCost, isAllowed)
        {
        }
    }
}
