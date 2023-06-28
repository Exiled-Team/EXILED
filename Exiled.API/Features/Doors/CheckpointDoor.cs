// -----------------------------------------------------------------------
// <copyright file="CheckpointDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;
    using System.Linq;

    using BaseDoor = Interactables.Interobjects.CheckpointDoor;

    /// <summary>
    /// Represents a Checkpoint door.
    /// </summary>
    public class CheckpointDoor : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckpointDoor"/> class.
        /// </summary>
        /// <param name="door">.</param>
        /// <param name="room">..</param>
        public CheckpointDoor(BaseDoor door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the original <see cref="BaseDoor"/>.
        /// </summary>
        public new BaseDoor Base { get; }

        /// <summary>
        /// Gets the list of all sub doors at current checkpoint.
        /// </summary>
        public IEnumerable<Door> SubDoors => Base.SubDoors.Select(Get);

        /// <summary>
        /// Gets or sets the current state of <see cref="CheckpointDoor"/>.
        /// </summary>
        public BaseDoor.CheckpointSequenceStage CurrentStage
        {
            get => Base._currentSequence;
            set => Base._currentSequence = value;
        }

        /// <summary>
        /// Gets or sets a time which checkpoint will wait until closing.
        /// </summary>
        public float WaitTime
        {
            get => Base._waitTime;
            set => Base._waitTime = value;
        }

        /// <summary>
        /// Gets or sets a time which checkpoint doors will be opened.
        /// </summary>
        public float OpeningTime
        {
            get => Base._openingTime;
            set => Base._openingTime = value;
        }
    }
}