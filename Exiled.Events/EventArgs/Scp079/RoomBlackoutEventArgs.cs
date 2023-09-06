// -----------------------------------------------------------------------
// <copyright file="RoomBlackoutEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    using MapGeneration;

    /// <summary>
    ///     Contains all information before SCP-079 turns off the lights in a room.
    /// </summary>
    public class RoomBlackoutEventArgs : IScp079Event, IRoomEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoomBlackoutEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="roomIdentifier">
        ///     <inheritdoc cref="Room" />
        /// </param>
        /// <param name="blackoutduration">
        ///     <inheritdoc cref="BlackoutDuration" />
        /// </param>
        /// <param name="auxiliaryPowerCost">
        ///     <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="cooldown">
        ///     <inheritdoc cref="Cooldown" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public RoomBlackoutEventArgs(ReferenceHub player, RoomIdentifier roomIdentifier, float auxiliaryPowerCost, float blackoutduration, float cooldown, bool isAllowed)
        {
            Player = Player.Get(player);
            Scp079 = Player.Role.As<Scp079Role>();
            Room = Room.Get(roomIdentifier);
            AuxiliaryPowerCost = auxiliaryPowerCost;
            BlackoutDuration = blackoutduration;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        ///     Gets the room that will be locked down.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        ///     Gets or sets the duration of the blackout.
        /// </summary>
        public float BlackoutDuration { get; set; }

        /// <summary>
        ///     Gets or sets the amount of auxiliary power required to black out the room.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        ///     Gets or sets the blackout cooldown duration.
        /// </summary>
        public double Cooldown { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-079 can black out the room.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}