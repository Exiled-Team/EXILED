// -----------------------------------------------------------------------
// <copyright file="BreakableDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Represents <see cref="Interactables.Interobjects.BreakableDoor"/>.
    /// </summary>
    public class BreakableDoor : BasicDoor, Interfaces.IDamageableDoor, Interfaces.INonInteractableDoor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreakableDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.BreakableDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public BreakableDoor(Interactables.Interobjects.BreakableDoor door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.BreakableDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.BreakableDoor Base { get; }

        /// <summary>
        /// Gets the prefab of broken door.
        /// </summary>
        public BrokenDoor BrokenDoorPrefab => Base._brokenPrefab;

        /// <summary>
        /// Gets or sets max health of the door.
        /// </summary>
        public new float MaxHealth
        {
            get => Base.MaxHealth;
            set => Base.MaxHealth = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not door is destroyed.
        /// </summary>
        public bool IsDestroyed
        {
            get => Base.Network_destroyed;
            set => Base.Network_destroyed = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not this door is breakable.
        /// </summary>
        public new bool IsBreakable => !IsDestroyed;

        /// <summary>
        /// Gets or sets remaining health of the door.
        /// </summary>
        public new float Health
        {
            get => Base.RemainingHealth;
            set => Base.RemainingHealth = value;
        }

        /// <summary>
        /// Gets or sets damage types which will be ignored.
        /// </summary>
        public DoorDamageType IgnoredDamage
        {
            get => Base._ignoredDamageSources;
            set => Base._ignoredDamageSources = value;
        }

        /// <inheritdoc/>
        public bool IgnoreLockdowns
        {
            get => Base._nonInteractable;
            set => Base._nonInteractable = value;
        }

        /// <inheritdoc/>
        public bool IgnoreRemoteAdmin
        {
            get => Base._nonInteractable;
            set => Base._nonInteractable = value;
        }

        /// <summary>
        /// Damages the door.
        /// </summary>
        /// <param name="amount">Amount to be dealt.</param>
        /// <param name="damageType">Damage type. Some types can be ignored according to <see cref="IgnoredDamage"/>.</param>
        /// <returns><see langword="true"/> if door was damaged. Otherwise, false.</returns>
        public bool Damage(float amount, DoorDamageType damageType = DoorDamageType.ServerCommand) => Base.ServerDamage(amount, damageType);

        /// <summary>
        /// Breaks the specified door. No effect if the door cannot be broken, or if it is already broken.
        /// </summary>
        /// <param name="type">The <see cref="DoorDamageType"/> to apply to the door.</param>
        /// <returns><see langword="true"/> if the door was broken, <see langword="false"/> if it was unable to be broken, or was already broken before.</returns>
        public bool Break(DoorDamageType type = DoorDamageType.ServerCommand) => Damage(float.MaxValue, type);

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} |{Health}/{MaxHealth}| -{IgnoredDamage}- *{IsDestroyed}*";
    }
}