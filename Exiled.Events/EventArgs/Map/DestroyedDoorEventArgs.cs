// -----------------------------------------------------------------------
// <copyright file="DestroyedDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    ///     Contains all the information after the door explodes.
    /// </summary>
    public class DestroyedDoorEventArgs : IDoorEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DestroyedDoorEventArgs" /> class.
        /// </summary>
        /// <param name="doorVariant">
        ///     <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="doorDamageType">
        ///     <inheritdoc cref="Interactables.Interobjects.DoorUtils.DoorDamageType" />
        /// </param>
        public DestroyedDoorEventArgs(DoorVariant doorVariant, DoorDamageType doorDamageType)
        {
            Door = API.Features.Doors.Door.Get(doorVariant);
            DamageType = doorDamageType;
        }

        /// <summary>
        ///     Gets a value indicating the door that was destroyed.
        /// </summary>
        public API.Features.Doors.Door Door { get; }

        /// <summary>
        /// Gets the <see cref="DoorDamageType"/>.
        /// </summary>
        public DoorDamageType DamageType { get; }
    }
}
