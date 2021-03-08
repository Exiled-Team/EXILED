// -----------------------------------------------------------------------
// <copyright file="ChangingIntoGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information for when the server is turning a pickup into a live grenade.
    /// </summary>
    public class ChangingIntoGrenadeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingIntoGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> being changed.</param>
        /// <param name="fuseTime">The duration, in seconds, of the fuse time to be used when the grenade goes live.</param>
        /// <param name="isAllowed">Whether or not the event is allowed to continue.</param>
        public ChangingIntoGrenadeEventArgs(Pickup pickup, float fuseTime = 3f, bool isAllowed = true)
        {
            Pickup = pickup;
            Type = pickup.itemId;
            FuseTime = fuseTime;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating the pickup being changed.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets a value indicating what type of grenade will be spawned.
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup will be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the fuse of the changed grenade will be.
        /// </summary>
        public float FuseTime { get; set; }
    }
}
