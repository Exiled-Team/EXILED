// -----------------------------------------------------------------------
// <copyright file="InteractingDoorEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains all informations before a player interacts with a door.
    /// </summary>
    public class InteractingDoorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingDoorEventArgs(Player player, DoorVariant door, bool isAllowed = true)
        {
            Player = player;
            Door = Door.Get(door);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.Door"/> instance.
        /// </summary>
        public Door Door { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with the door.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
