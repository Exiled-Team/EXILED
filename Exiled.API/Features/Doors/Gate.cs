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
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;

    /// <summary>
    /// Represents a "pryable" door or gate.
    /// </summary>
    public class Gate : BasicDoor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gate"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="PryableDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/>'s for this door.</param>
        public Gate(PryableDoor door, List<Room> room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the base <see cref="PryableDoor"/>.
        /// </summary>
        public new PryableDoor Base { get; }

        /// <summary>
        /// Gets the list of all available pry positions.
        /// </summary>
        public IEnumerable<Transform> PryPositions => Base.PryPositions;

        /// <summary>
        /// Gets a value indicating whether or not the door is fully closed.
        /// </summary>
        public override bool IsFullyClosed => base.IsFullyClosed && RemainingPryCooldown <= 0;

        /// <summary>
        /// Gets a value indicating whether the door is fully open.
        /// </summary>
        public override bool IsFullyOpen => base.IsFullyOpen || (Base is Timed173PryableDoor && ExactState is 0.5845918f);

        /// <summary>
        /// Gets a value indicating whether or not the door is currently moving.
        /// </summary>
        public override bool IsMoving => base.IsMoving || RemainingPryCooldown > 0;

        /// <summary>
        /// Gets or sets remaining cooldown for prying.
        /// </summary>
        public float RemainingPryCooldown
        {
            get => Base._remainingPryCooldown;
            set => Base._remainingPryCooldown = value;
        }

        /// <summary>
        /// Gets or sets <see cref="DoorLockType"/> which will block prying if door has them.
        /// </summary>
        public DoorLockType BlockingPryingMask
        {
            get => (DoorLockType)Base._blockPryingMask;
            set => Base._blockPryingMask = (DoorLockReason)value;
        }

        /// <summary>
        /// Tries to pry the door open. No effect if the door cannot be pried.
        /// </summary>
        /// <returns><see langword="true"/> if the door was able to be pried open.</returns>
        /// <param name="player"><see cref="Player"/> to perform pry gate.</param>
        public bool TryPry(Player player = null) => Base.TryPryGate(player?.ReferenceHub);

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} |{BlockingPryingMask}| -{RemainingPryCooldown}-";
    }
}