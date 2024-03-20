// -----------------------------------------------------------------------
// <copyright file="DestroyingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.Events.EventArgs.Interfaces;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    ///     Contains all the information before the door explodes.
    /// </summary>
    public class DestroyingDoorEventArgs : IDeniableEvent, IDoorEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DestroyingDoorEventArgs" /> class.
        /// </summary>
        /// <param name="doorVariant">
        ///     <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DestroyingDoorEventArgs(DoorVariant doorVariant, bool isAllowed = true)
        {
            Door = API.Features.Doors.Door.Get(doorVariant);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the door can be destroyed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets a value indicating the door that will be destroyed.
        /// </summary>
        public API.Features.Doors.Door Door { get; }
    }
}
