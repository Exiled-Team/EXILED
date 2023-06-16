// -----------------------------------------------------------------------
// <copyright file="PingingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;

    using Exiled.API.Enums;

    using Interfaces;

    using RelativePositioning;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before SCP-079 pings a location.
    /// </summary>
    public class PingingEventArgs : IPlayerEvent, IRoomEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PingingEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        /// <param name="proccesorindex">
        ///     <inheritdoc cref="Type" />
        /// </param>
        /// <param name="powerCost">
        ///     <inheritdoc cref="AuxiliaryPowerCost" />
        /// </param>
        /// <param name="syncNormal">
        ///     <inheritdoc cref="SyncNormal" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PingingEventArgs(ReferenceHub hub, RelativePosition position, int powerCost, byte proccesorindex, Vector3 syncNormal, bool isAllowed = true)
        {
            Player = Player.Get(hub);
            Position = position.Position;
            Room = Room.Get(Position);
            AuxiliaryPowerCost = powerCost;
            Type = (PingType)proccesorindex;
            SyncNormal = syncNormal;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating if the sync matches.
        /// </summary>
        public Vector3 SyncNormal { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets or sets the amount of auxiliary power required for SCP-079 to ping.
        /// </summary>
        public float AuxiliaryPowerCost { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the type of ping.
        /// </summary>
        public PingType Type { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating the position of the ping.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <inheritdoc />
        public Room Room { get; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}