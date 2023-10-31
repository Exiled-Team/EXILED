// -----------------------------------------------------------------------
// <copyright file="StoppingSpeakerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 finishes using a speaker.
    /// </summary>
    public class StoppingSpeakerEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StoppingSpeakerEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="room">
        ///     <inheritdoc cref="Room" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public StoppingSpeakerEventArgs(Player player, Room room, bool isAllowed = true)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Room = room;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        ///     Gets the room that the speaker is located in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-079 can stop using the speaker.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}