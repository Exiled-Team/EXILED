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
    using UnityEngine;

    /// <summary>
    /// Contains all informations before SCP-079 lockdowns a room.
    /// </summary>
    public class LockingDownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockingDownEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="roomGameObject"><inheritdoc cref="RoomGameObject"/></param>
        /// <param name="auxiliaryPowerCost"><inheritdoc cref="AuxiliaryPowerCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public LockingDownEventArgs(Player player, GameObject roomGameObject, float auxiliaryPowerCost, bool isAllowed = true)
        {
            Player = player;
            RoomGameObject = roomGameObject;
            AuxiliaryPowerCost = auxiliaryPowerCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the gameobject of the room that will be lockdowned.
        /// </summary>
        public GameObject RoomGameObject { get; set; }

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
