// -----------------------------------------------------------------------
// <copyright file="Scp244SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using API.Features;
    using Exiled.API.Features.Pickups;
    using Interfaces;

    /// <summary>
    ///     Contains all information up to spawning Scp244.
    /// </summary>
    public class Scp244SpawningEventArgs : IRoomEvent, IPickupEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Scp244SpawningEventArgs" /> class.
        /// </summary>
        /// <param name="room">
        ///     <inheritdoc cref="Room" />
        /// </param>
        /// <param name="scp244Pickup">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public Scp244SpawningEventArgs(Room room, Scp244Pickup scp244Pickup, bool isAllowed = true)
        {
            Room = room;
            Pickup = scp244Pickup;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the room in which the Pickup will be spawning.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets a value indicating the pickup being spawning.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the item can be spawning.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}