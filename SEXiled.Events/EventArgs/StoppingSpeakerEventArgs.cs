// -----------------------------------------------------------------------
// <copyright file="StoppingSpeakerEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 finishes using a speaker.
    /// </summary>
    public class StoppingSpeakerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoppingSpeakerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="room"><inheritdoc cref="Room"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StoppingSpeakerEventArgs(Player player, Room room, bool isAllowed = true)
        {
            Player = player;
            Room = room;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the room that the speaker is located in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can stop using the speaker.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
