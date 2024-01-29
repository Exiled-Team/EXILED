// -----------------------------------------------------------------------
// <copyright file="StartingSpeakerEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before SCP-079 uses a speaker.
    /// </summary>
    public class StartingSpeakerEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartingSpeakerEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="room">
        ///     <inheritdoc cref="Room" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        ///     <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public StartingSpeakerEventArgs(Player player, Room room, float auxiliaryPowerCost, bool isAllowed = true)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            Room = room;
            AuxiliaryPowerCost = auxiliaryPowerCost;
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
        ///     Gets or sets the amount of auxiliary power required to use a speaker through SCP-079.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-079 can use the speaker.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}