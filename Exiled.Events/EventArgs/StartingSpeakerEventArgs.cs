// -----------------------------------------------------------------------
// <copyright file="StartingSpeakerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 uses a speaker.
    /// </summary>
    public class StartingSpeakerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingSpeakerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="room"><inheritdoc cref="Room"/></param>
        /// <param name="apDrain"><inheritdoc cref="APDrain"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StartingSpeakerEventArgs(Player player, Room room, float apDrain, bool isAllowed = true)
        {
            Player = player;
            Room = room;
            APDrain = apDrain;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's triggering the speaker through SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the room where the camera is located, that SCP-079 is triggering.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets the amount of AP that will be removed for the first time when using speakers through SCP-079.
        /// </summary>
        public float APDrain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
