// -----------------------------------------------------------------------
// <copyright file="BasicDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;

    using UnityEngine;

    using Basegame = Interactables.Interobjects.BasicDoor;

    /// <summary>
    /// Represents a basic interactable door.
    /// </summary>
    public class BasicDoor : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.BasicDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public BasicDoor(Basegame door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the base <see cref="Basegame"/>.
        /// </summary>
        public new Basegame Base { get; }

        /// <summary>
        /// Gets the list with all SCP-106's colliders.
        /// </summary>
        public IEnumerable<Collider> Scp106Colliders => Base.Scp106Colliders;

        /// <summary>
        /// Gets or sets total cooldown before door can be triggered again.
        /// </summary>
        public float Cooldown
        {
            get => Base._cooldownDuration;
            set => Base._cooldownDuration = value;
        }

        /// <summary>
        /// Gets or sets the remaining cooldown before door can be triggered again.
        /// </summary>
        public float RemainingCooldown
        {
            get => Base._remainingAnimCooldown;
            set => Base._remainingAnimCooldown = value;
        }
    }
}