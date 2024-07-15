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
    /// Contains all information up to spawning Scp244.
    /// </summary>
    public class Scp244SpawningEventArgs : IRoomEvent, IPickupEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244SpawningEventArgs" /> class.
        /// </summary>
        /// <param name="room">
        /// <inheritdoc cref="Room" />
        /// </param>
        /// <param name="scp244Pickup">
        /// <inheritdoc cref="Pickup" />
        /// </param>
        public Scp244SpawningEventArgs(Room room, Pickup scp244Pickup)
        {
            Room = room;
            Pickup = scp244Pickup;
            Scp244Pickup = scp244Pickup.As<Scp244Pickup>();
        }

        /// <summary>
        /// Gets the <see cref="Room"/> which the Pickup will be spawning in.
        /// </summary>
        public Room Room { get; }

        /// <inheritdoc />
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets a value indicating the pickup being spawning.
        /// </summary>
        public Scp244Pickup Scp244Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item can be spawning.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}