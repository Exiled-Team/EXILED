// -----------------------------------------------------------------------
// <copyright file="TriggeringDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    using Player;

    /// <summary>
    /// Contains all information before SCP-079 interacts with a door.
    /// </summary>
    public class TriggeringDoorEventArgs : IScp079Event, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringDoorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="InteractingDoorEventArgs.Player" />
        /// </param>
        /// <param name="door">
        /// <inheritdoc cref="InteractingDoorEventArgs.Door" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        /// <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        public TriggeringDoorEventArgs(Player player, DoorVariant door, float auxiliaryPowerCost)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Door = Door.Get(door);
            AuxiliaryPowerCost = auxiliaryPowerCost;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the door.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="API.Features.Doors.Door" /> instance.
        /// </summary>
        public Door Door { get; set; }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to trigger a door through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }
    }
}