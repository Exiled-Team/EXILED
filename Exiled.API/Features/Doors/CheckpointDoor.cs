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

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Represents a checkpoint door.
    /// </summary>
    public class CheckpointDoor : Door, Interfaces.IDamageableDoor
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
            Subdoors = new List<BreakableDoor>(Base.SubDoors.Select(x => Get(x).As<BreakableDoor>()));
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.CheckpointDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.CheckpointDoor Base { get; }

        /// <summary>
        /// Gets the list of all sub doors for this <see cref="CheckpointDoor"/>.
        /// </summary>
        public IReadOnlyCollection<BreakableDoor> Subdoors { get; }

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

        /// <inheritdoc/>
        public bool IsDestroyed
        {
            get => Base.IsDestroyed;
            set => Base.IsDestroyed = value;
        }

        /// <inheritdoc/>
        public new bool IsBreakable => !IsDestroyed;

        /// <inheritdoc/>
        public new float Health
        {
            get => Base.GetHealthPercent();
            set { }
        }

        /// <inheritdoc/>
        public new float MaxHealth
        {
            get => Subdoors.Sum(door => door.MaxHealth);
            set
            {
                foreach (var door in Subdoors)
                {
                    door.MaxHealth = value;
                }
            }
        }

        /// <inheritdoc/>
        public DoorDamageType IgnoredDamage
        {
            get => Subdoors.Aggregate(DoorDamageType.None, (current, door) => current | door.IgnoredDamage);
            set
            {
                foreach (var door in Subdoors)
                {
                    door.IgnoredDamage = value;
                }
            }
        }

        /// <summary>
        /// Toggles the state of the doors from <see cref="Subdoors"/>.
        /// </summary>
        /// <param name="newState">New state for the subdoors.</param>
        public void ToggleAllDoors(bool newState) => Base.ToggleAllDoors(newState);

        /// <inheritdoc/>
        public bool Damage(float amount, DoorDamageType damageType = DoorDamageType.ServerCommand) => Base.ServerDamage(amount, damageType);

        /// <inheritdoc/>
        public bool Break(DoorDamageType type = DoorDamageType.ServerCommand) => Base.ServerDamage(float.MaxValue, type);

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} |{WaitTime}| -{WarningTime}-";
    }
}