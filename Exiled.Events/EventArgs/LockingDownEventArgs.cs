// -----------------------------------------------------------------------
// <copyright file="LockingDownEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;

    using MapGeneration;

    /// <summary>
    /// Contains all information before SCP-079 lockdowns a room.
    /// </summary>
    public class LockingDownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockingDownEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="roomIdentifier"><inheritdoc cref="RoomGameObject"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        public LockingDownEventArgs(Player player, RoomIdentifier roomIdentifier, float auxiliaryPowerCost)
        {
            Player = player;
            RoomGameObject = roomIdentifier;
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = auxiliaryPowerCost <= player.Role.As<Scp079Role>().Energy;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="RoomIdentifier"/> of the room that will be locked down.
        /// </summary>
        public RoomIdentifier RoomGameObject { get; }

        /// <summary>
        /// Gets or sets the amount of auxiliary power required to lockdown a room.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can lockdown a room.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
