// -----------------------------------------------------------------------
// <copyright file="TriggeringDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains all informations before SCP-079 interacts with a door.
    /// </summary>
    public class TriggeringDoorEventArgs : InteractingDoorEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="InteractingDoorEventArgs.Player"/></param>
        /// <param name="door"><inheritdoc cref="InteractingDoorEventArgs.Door"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="InteractingDoorEventArgs.IsAllowed"/></param>
        public TriggeringDoorEventArgs(Player player, DoorVariant door, float auxiliaryPowerCost, bool isAllowed = true)
            : base(player, door, isAllowed)
        {
            AuxiliaryPowerCost = auxiliaryPowerCost;
        }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to trigger a door through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }
    }
}
