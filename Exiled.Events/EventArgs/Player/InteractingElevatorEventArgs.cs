// -----------------------------------------------------------------------
// <copyright file="InteractingElevatorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs.Interfaces;

    using Lift = Lift;

    /// <summary>
    ///     Contains all information before a player interacts with an elevator.
    /// </summary>
    public class InteractingElevatorEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InteractingElevatorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="elevator">
        ///     <inheritdoc cref="Elevator" />
        /// </param>
        /// <param name="lift">
        ///     <inheritdoc cref="Type" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingElevatorEventArgs(Player player, Lift.Elevator elevator, Lift lift, bool isAllowed = true)
        {
            Lift = API.Features.Lift.Get(lift);
            Player = player;
            Elevator = Lift.Elevators.FirstOrDefault(elev => elev.Base == elevator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="API.Structs.Elevator" /> instance.
        /// </summary>
        public Elevator Elevator { get; }

        /// <summary>
        ///     Gets the <see cref="Lift" /> instance.
        /// </summary>
        public API.Features.Lift Lift { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can interact with the elevator.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's interacting with the elevator.
        /// </summary>
        public Player Player { get; }
    }
}