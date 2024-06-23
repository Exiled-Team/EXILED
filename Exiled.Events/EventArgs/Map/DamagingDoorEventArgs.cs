// -----------------------------------------------------------------------
// <copyright file="DamagingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Doors;
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    ///     Contains all information before the door is damaged.
    /// </summary>
    public class DamagingDoorEventArgs : IDoorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DamagingDoorEventArgs" /> class.
        /// </summary>
        /// <param name="doorVariant">
        ///     <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="damageAmount">
        ///     <inheritdoc cref="float" />
        /// </param>
        /// <param name="doorDamageType">
        ///     <inheritdoc cref="Interactables.Interobjects.DoorUtils.DoorDamageType" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DamagingDoorEventArgs(DoorVariant doorVariant, float damageAmount, DoorDamageType doorDamageType, bool isAllowed = true)
        {
            Door = Door.Get(doorVariant);
            DamageAmount = damageAmount;
            DoorDamageType = doorDamageType;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the door can take damage.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///    Gets or sets a value indicating the type of door damage (DoorDamageType).
        /// </summary>
        public DoorDamageType DoorDamageType { get; set; }

        /// <summary>
        ///     Gets or sets the damage dealt to the door.
        /// </summary>
        public float DamageAmount { get; set; }

        /// <summary>
        ///     Gets a value indicating the door that takes damage.
        /// </summary>
        public Door Door { get; }
    }
}
