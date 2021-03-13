// -----------------------------------------------------------------------
// <copyright file="InteractingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains all informations before a player interacts with a door.
    /// </summary>
    public class InteractingDoorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingDoorEventArgs(Player player, DoorVariant door, float auxiliaryPowerCost, bool isAllowed = true)
        {
            Player = player;
            Door = door;
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="Door"/> instance.
        /// </summary>
        public DoorVariant Door { get; set; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to interact with a tesla gate.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with the door.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
