// -----------------------------------------------------------------------
// <copyright file="ThrowKeycardInteractingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Keycards;

    /// <summary>
    /// Contains all informations before a keycard interacts with a door.
    /// </summary>
    public class ThrowKeycardInteractingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowKeycardInteractingEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ThrowKeycardInteractingEventArgs(KeycardPickup pickup, Player player, DoorVariant door, bool isAllowed = true)
        {
            Pickup = Pickup.Get(pickup);
            Player = player;
            Door = Door.Get(door);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the pickup that's interacting with the door.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Door"/> instance.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the keycard can interact with the door.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
