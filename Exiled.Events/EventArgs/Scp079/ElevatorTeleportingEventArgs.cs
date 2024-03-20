// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features.Doors;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    using MapGeneration;

    using ElevatorDoor = API.Features.Doors.ElevatorDoor;

    /// <summary>
    /// Contains all information before SCP-079 changes rooms via elevator.
    /// </summary>
    public class ElevatorTeleportingEventArgs : IScp079Event, IRoomEvent, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorTeleportingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="room">
        /// <inheritdoc cref="Room" />
        /// </param>
        /// <param name="elevatorDoor">
        /// <inheritdoc cref="Lift" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        /// <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        public ElevatorTeleportingEventArgs(Player player, RoomIdentifier room, Interactables.Interobjects.ElevatorDoor elevatorDoor, float auxiliaryPowerCost)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Room = Room.Get(room);
            ElevatorDoor = Door.Get(elevatorDoor).As<ElevatorDoor>();
            Lift = ElevatorDoor.Lift;
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = auxiliaryPowerCost <= Scp079.Energy;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to teleport to an elevator camera.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <inheritdoc/>
        public Room Room { get; }

        /// <summary>
        /// Gets the <see cref="Lift" /> SCP-079 wants to move.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets the <see cref="ElevatorDoor" /> SCP-079 wants to move.
        /// </summary>
        public ElevatorDoor ElevatorDoor { get; }

        /// <inheritdoc/>
        public Door Door => ElevatorDoor;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can teleport.
        /// Defaults to a <see cref="bool" /> describing whether or not SCP-079 has enough auxiliary power to teleport.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}