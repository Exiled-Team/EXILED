// -----------------------------------------------------------------------
// <copyright file="DoorDamagingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Door
{
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.API.Features.Doors;
    using Interactables.Interobjects.DoorUtils;

    public class DoorDamagingEventArgs : IDoorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoorDamagingEventArgs" /> class.
        /// </summary>
        /// <param name="doorVariant">
        ///     <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="hp">
        ///     <inheritdoc cref="float" />
        /// </param>
        /// <param name="doorDamageType">
        ///     <inheritdoc cref="Interactables.Interobjects.DoorUtils.DoorDamageType" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DoorDamagingEventArgs(DoorVariant doorVariant, float hp, DoorDamageType doorDamageType, bool isAllowed = true)
        {
            Door = Door.Get(doorVariant);
            Hp = hp;
            DoorDamageType = doorDamageType;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the door can take damage
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///    Gets or sets a value indicating the type of door damage (DoorDamageType)
        /// </summary>
        public DoorDamageType DoorDamageType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the damage received by the door
        /// </summary>
        public float Hp { get; set; }

        /// <summary>
        ///     Gets a value indicating the door that takes damage
        /// </summary>
        public Door Door { get; }
    }
}
