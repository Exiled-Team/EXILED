// -----------------------------------------------------------------------
// <copyright file="InteractingElevatorEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;
    using System.Linq;

    using SEXiled.API.Features;
    using SEXiled.API.Structs;

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
        public InteractingElevatorEventArgs(Player player, global::Lift.Elevator elevator, global::Lift lift, bool isAllowed = true)
        {
            Lift = Lift.Get(lift);
            Player = player;
            Elevator = Lift.Elevators.FirstOrDefault(elev => elev.Base == elevator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the elevator.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Structs.Elevator"/> instance.
        /// </summary>
        public Elevator Elevator { get; }

        /// <summary>
        /// Gets the <see cref="Lift"/> instance.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can interact with the elevator.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
