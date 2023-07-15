// -----------------------------------------------------------------------
// <copyright file="Gate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    using BaseDoor = Interactables.Interobjects.PryableDoor;

    /// <summary>
    /// Represents a Gate door.
    /// </summary>
    public class Gate : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gate"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="BaseDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public Gate(BaseDoor door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <inheritdoc cref="Door.Base"/>
        public new BaseDoor Base { get; }

        /// <summary>
        /// Gets the list of all available pry positions.
        /// </summary>
        public IEnumerable<Transform> PryPositions => Base.PryPositions;

        /// <summary>
        /// Gets or sets a cooldown for prying gate.
        /// </summary>
        public float RemainingPryCooldown
        {
            get => Base._remainingPryCooldown;
            set => Base._remainingPryCooldown = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="DoorLockType"/> which will block pry if door has it.
        /// </summary>
        public DoorLockType BlockPrying
        {
            get => (DoorLockType)Base._blockPryingMask;
            set => Base._blockPryingMask = (DoorLockReason)value;
        }

        /// <summary>
        /// Tries to pry the door open.
        /// </summary>
        /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
        /// <param name="player">The <see cref="Player"/> who will pry the gate.</param>
        public bool TryPry(Player player) => Base.TryPryGate(player is null ? Server.Host.ReferenceHub : player.ReferenceHub);

        /// <summary>
        /// Tries to pry the door open.
        /// </summary>
        /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
        public bool TryPry() => TryPry(null);
    }
}