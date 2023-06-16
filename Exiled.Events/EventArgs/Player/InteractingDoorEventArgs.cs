// -----------------------------------------------------------------------
// <copyright file="InteractingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interactables.Interobjects.DoorUtils;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player interacts with a door.
    /// </summary>
    public class InteractingDoorEventArgs : IPlayerEvent, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InteractingDoorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="door">
        ///     <inheritdoc cref="Door" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingDoorEventArgs(Player player, DoorVariant door, bool isAllowed = true)
        {
            Player = player;
            Door = Door.Get(door);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Door Door { get; set; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}
