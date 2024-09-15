// -----------------------------------------------------------------------
// <copyright file="TrackablesGC.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Interfaces;

    /// <summary>
    /// Represents a class responsible for handling the garbage collection of <see cref="ITrackable"/> objects.
    /// It maintains a collection of tracked objects identified by their unique serial numbers,
    /// and handles allocation, collection, and sweeping of these objects.
    /// </summary>
    public sealed class TrackablesGC : StaticActor<TrackablesGC>
    {
#pragma warning disable SA1309
        /// <summary>
        /// Stores the serial numbers of allocated objects.
        /// </summary>
        private HashSet<ushort> allocated = new();

        /// <summary>
        /// Indicates whether the garbage collection process is paused.
        /// </summary>
        private bool isCollectionPaused;
#pragma warning restore SA1309

        /// <summary>
        /// Gets the current count of tracked objects.
        /// </summary>
        public int TrackedCount => allocated.Count;

        /// <summary>
        /// Allocates a new serial number for tracking.
        /// </summary>
        /// <param name="serial">The serial number to allocate.</param>
        /// <returns><see langword="true"/> if allocation is successful; otherwise, <see langword="false"/>.</returns>
        public bool Allocate(ushort serial) => allocated.Add(serial);

        /// <summary>
        /// Frees the allocated object associated with the specified serial number.
        /// </summary>
        /// <param name="serial">The serial number of the object to be freed.</param>
        /// <returns>
        /// <see langword="true"/> if the object was successfully freed (i.e., the serial number was found and removed from the allocated set);
        /// otherwise, <see langword="true"/>.
        /// </returns>
        public bool Free(ushort serial) => allocated.Remove(serial);

        /// <summary>
        /// Attempts to collect and remove an object based on its serial number.
        /// </summary>
        /// <param name="serial">The serial number of the object to collect.</param>
        /// <returns><see langword="true"/> if the object is collected successfully; <see langword="false"/> if collection is paused or the object is not found.</returns>
        public bool Collect(ushort serial)
        {
            if (isCollectionPaused || !IsAllocated(serial))
                return false;

            Item item = Item.Get(serial);
            if (item is not null && item.Owner is not null && item.Owner == Server.Host)
            {
                Sweep(item, serial);
                return true;
            }

            Pickup pickup = Pickup.Get(serial);
            if (pickup is not null)
            {
                Sweep(pickup, serial);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sweeps an object, removing it from tracking and its associated components.
        /// </summary>
        /// <param name="entity">The <see cref="GameEntity"/> to sweep.</param>
        /// <param name="serial">The serial number of the object.</param>
        /// <returns>The serial number if the sweep is successful, or 0 if the object is not found.</returns>
        public ushort Sweep(GameEntity entity, ushort serial)
        {
            if (!entity.Is(out Item _) && !entity.Is(out Pickup _))
                return 0;

            allocated.Remove(serial);
            return serial;
        }

        /// <summary>
        /// Triggers the garbage collection process for all tracked objects.
        /// </summary>
        public void TriggerCollection() => allocated.ForEach(fc => Collect(fc));

        /// <summary>
        /// Pauses the garbage collection process.
        /// </summary>
        public void PauseCollection() => isCollectionPaused = true;

        /// <summary>
        /// Resumes the garbage collection process if it was paused.
        /// </summary>
        public void ResumeCollection() => isCollectionPaused = false;

        /// <summary>
        /// Checks whether a specific object is currently allocated by its serial number.
        /// </summary>
        /// <param name="serial">The serial number to check.</param>
        /// <returns><see langword="true"/> if the object is currently allocated by its serial number; otherwise, <see langword="false"/>.</returns>
        public bool IsAllocated(ushort serial) => allocated.Contains(serial);
    }
}
