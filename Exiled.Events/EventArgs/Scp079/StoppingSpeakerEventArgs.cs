// -----------------------------------------------------------------------
// <copyright file="StoppingSpeakerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 finishes using a speaker.
    /// </summary>
    public class StoppingSpeakerEventArgs : IPlayerEvent, IRoomEvent, IDeniableEvent
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
            Room = room;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Room Room { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}