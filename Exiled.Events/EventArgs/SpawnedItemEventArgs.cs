// -----------------------------------------------------------------------
// <copyright file="SpawnedItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations after the server spawns an item.
    /// </summary>
    public class SpawnedItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnedItemEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        public SpawnedItemEventArgs(Pickup pickup)
        {
            Pickup = pickup;
        }

        /// <summary>
        /// Gets or sets the item pickup.
        /// </summary>
        public Pickup Pickup { get; set; }
    }
}
