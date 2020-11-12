// -----------------------------------------------------------------------
// <copyright file="SpawningItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before the server spawns an item.
    /// </summary>
    public class SpawningItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningItemEventArgs"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="pos"><inheritdoc cref="Position"/></param>
        /// <param name="rot"><inheritdoc cref="Rotation"/></param>
        /// <param name="locked"><inheritdoc cref="Locked"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningItemEventArgs(ItemType id, Vector3 pos, Quaternion rot, bool locked, bool isAllowed = true)
        {
            Id = id;
            Position = pos;
            Rotation = rot;
            Locked = locked;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the item to be dropped.
        /// </summary>
        public ItemType Id { get; set; }

        /// <summary>
        /// Gets or sets the position to spawn the item.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation to spawn the item.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Pickup will be locked.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
