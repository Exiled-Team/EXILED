// -----------------------------------------------------------------------
// <copyright file="InteractingElevatorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player interacts with an elevator.
    /// </summary>
    public class InteractingElevatorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingElevatorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="elevator"><inheritdoc cref="Elevator"/></param>
        /// <param name="lift"><inheritdoc cref="Type"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingElevatorEventArgs(Player player, Lift.Elevator elevator, Lift lift, bool isAllowed = true)
        {
            Status = lift.status;
            Player = player;
            Elevator = elevator;
            IsAllowed = isAllowed;
#pragma warning disable CS0618 // Type or member is obsolete
            Type = lift.Type();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Gets the player who's interacting with the elevator.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Lift.Elevator"/> instance.
        /// </summary>
        public Lift.Elevator Elevator { get; }

        /// <summary>
        /// Gets the <see cref="Lift"/> current <see cref="Lift.Status"/>.
        /// </summary>
        public Lift.Status Status { get; }

        /// <summary>
        /// Gets the <see cref="ElevatorType"/>.
        /// </summary>
        [Obsolete("Use Lift.Type() extension method instead.")]
        public ElevatorType Type { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with the elevator.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
