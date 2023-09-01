// -----------------------------------------------------------------------
// <copyright file="BasicNonInteractableDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using Exiled.API.Interfaces;

    using Basegame = Interactables.Interobjects.BasicNonInteractableDoor;

    /// <summary>
    /// Represents a basic non-interactable door.
    /// </summary>
    public class BasicNonInteractableDoor : BasicDoor, INonInteractableDoor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicNonInteractableDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.BasicNonInteractableDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public BasicNonInteractableDoor(Basegame door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the base <see cref="Basegame"/>.
        /// </summary>
        public new Basegame Base { get; }

        /// <inheritdoc/>
        public bool IgnoreLockdowns
        {
            get => Base._ignoreLockdowns;
            set => Base._ignoreLockdowns = value;
        }

        /// <inheritdoc/>
        public bool IgnoreRemoteAdmin
        {
            get => Base._ignoreRemoteAdmin;
            set => Base._ignoreRemoteAdmin = value;
        }
    }
}