// -----------------------------------------------------------------------
// <copyright file="ChangingSpeakerStatusEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;

    using Exiled.API.Features.Roles;

    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 uses a speaker.
    /// </summary>
    public class ChangingSpeakerStatusEventArgs : IPlayerEvent, IRoomEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingSpeakerStatusEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingSpeakerStatusEventArgs(Player player, bool isAllowed)
        {
            Player = player;
            Room = Room.Get(player.Role.As<Scp079Role>().Speaker.Room);
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