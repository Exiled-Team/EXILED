// -----------------------------------------------------------------------
// <copyright file="IDamageableDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Interface for doors that can be damaged.
    /// </summary>
    public interface IDamageableDoor
    {
        /// <summary>
        /// Gets or sets the health of the door.
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Gets or sets max health of the door.
        /// </summary>
        public float MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not door is destroyed.
        /// </summary>
        public bool IsDestroyed { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this door is breakable.
        /// </summary>
        public bool IsBreakable { get; }

        /// <summary>
        /// Gets or sets damage types which will be ignored.
        /// </summary>
        public DoorDamageType IgnoredDamage { get; set; }

        /// <summary>
        /// Damages the door.
        /// </summary>
        /// <param name="amount">Amount to be dealt.</param>
        /// <param name="damageType">Damage type. Some types can be ignored.</param>
        /// <returns><see langword="true"/> if door was damaged. Otherwise, false.</returns>
        public bool Damage(float amount, DoorDamageType damageType = DoorDamageType.ServerCommand);

        /// <summary>
        /// Breaks the specified door. No effect if the door cannot be broken, or if it is already broken.
        /// </summary>
        /// <param name="type">The <see cref="DoorDamageType"/> to apply to the door.</param>
        /// <returns><see langword="true"/> if the door was broken, <see langword="false"/> if it was unable to be broken, or was already broken before.</returns>
        public bool Break(DoorDamageType type = DoorDamageType.ServerCommand);
    }
}