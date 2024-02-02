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
        internal CheckpointDoor(Interactables.Interobjects.CheckpointDoor door, List<Room> room)
            : base(door, room)
        {
            Base = door;
            Subdoors = SubDoorsValue.AsReadOnly();
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.CheckpointDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.CheckpointDoor Base { get; }

        /// <summary>
        /// Gets the list of all sub doors belonging to this <see cref="CheckpointDoor"/>.
        /// </summary>
        public IReadOnlyCollection<BreakableDoor> Subdoors { get; }

        /// <summary>
        /// Gets or sets the current checkpoint stage.
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
        public bool IsBreakable => !IsDestroyed;

        /// <inheritdoc/>
        public float Health
        {
            get => Base.GetHealthPercent();
            set
            {
                float health = value / Subdoors.Count;

                foreach (BreakableDoor door in Subdoors)
                {
                    door.Health = health;
                }
            }
        }

        /// <inheritdoc/>
        public float MaxHealth
        {
            get => Subdoors.Sum(door => door.MaxHealth);
            set
            {
                float health = value / Subdoors.Count;

                foreach (BreakableDoor door in Subdoors)
                {
                    door.MaxHealth = health;
                }
            }
        }

        /// <inheritdoc/>
        public DoorDamageType IgnoredDamage
        {
            get => Subdoors.Aggregate(DoorDamageType.None, (current, door) => current | door.IgnoredDamage);
            set
            {
                foreach (BreakableDoor door in Subdoors)
                {
                    door.IgnoredDamage = value;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all known subdoors <see cref="Door"/>s.
        /// </summary>
        internal List<BreakableDoor> SubDoorsValue { get; } = new();

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