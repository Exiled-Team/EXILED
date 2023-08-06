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
    public class Gate : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gate"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.PryableDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public Gate(PryableDoor door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.PryableDoor"/>.
        /// </summary>
        public new PryableDoor Base { get; }

        /// <summary>
        /// Gets the list of all available pry positions.
        /// </summary>
        public IEnumerable<Transform> PryPositions => Base.PryPositions;

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

        /// <inheritdoc cref="Door.TryPryOpen(Player)"/>
        public bool TryPry(Player player = null) => Base.TryPryGate(player?.ReferenceHub);

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} |{BlockingPryingMask}| -{RemainingPryCooldown}-";
    }
}