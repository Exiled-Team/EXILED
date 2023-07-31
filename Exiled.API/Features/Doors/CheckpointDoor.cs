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

    /// <summary>
    /// Represents a check point door.
    /// </summary>
    public class CheckpointDoor : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckpointDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.CheckpointDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public CheckpointDoor(Interactables.Interobjects.CheckpointDoor door, Room room)
            : base(door, room)
        {
            Base = door;
            Subdoors = new List<Door>(Base.SubDoors.Select(Get));
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.CheckpointDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.CheckpointDoor Base { get; }

        /// <summary>
        /// Gets the list of all sub doors for this <see cref="CheckpointDoor"/>.
        /// </summary>
        public IReadOnlyCollection<Door> Subdoors { get; }

        /// <summary>
        /// Gets or sets the current stage.
        /// </summary>
        public Interactables.Interobjects.CheckpointDoor.CheckpointSequenceStage CurrentStage
        {
            get => Base._currentSequence;
            set => Base._currentSequence = value;
        }

        /// <summary>
        /// Gets or sets a time in seconds for main timer.
        /// </summary>
        public float MainTimer
        {
            get => Base._mainTimer;
            set => Base._mainTimer = value;
        }

        /// <summary>
        /// Gets or sets time before doors close.
        /// </summary>
        public float WaitTime
        {
            get => Base._waitTime;
            set => Base._waitTime = value;
        }

        /// <summary>
        /// Gets or sets time in seconds when warning will be shown.
        /// </summary>
        public float WarningTime
        {
            get => Base._warningTime;
            set => Base._warningTime = value;
        }

        /// <summary>
        /// Toggles all doors from <see cref="Subdoors"/>.
        /// </summary>
        /// <param name="newState">New state for doors.</param>
        public void ToggleAllDoors(bool newState) => Base.ToggleAllDoors(newState);
    }
}