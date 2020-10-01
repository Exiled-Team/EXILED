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
            Type = GetElevatorType(lift.elevatorName);
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
        public ElevatorType Type { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        private static ElevatorType GetElevatorType(string name)
        {
            switch (name)
            {
                case "":
                {
                    return ElevatorType.Nuke;
                }

                case "ElA":
                case "ElA2":
                {
                    return ElevatorType.LczA;
                }

                case "ElB":
                case "ElB2":
                {
                    return ElevatorType.LczB;
                }

                case "GateA":
                {
                    return ElevatorType.GateA;
                }

                case "GateB":
                {
                    return ElevatorType.GateB;
                }

                case "SCP-049":
                {
                    return ElevatorType.Scp049;
                }

                default:
                {
                    return ElevatorType.Unknown;
                }
            }
        }
    }
}
