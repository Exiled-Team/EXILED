// -----------------------------------------------------------------------
// <copyright file="InteractingElevatorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using Interactables.Interobjects;

    using Lift = API.Features.Lift;

    /// <summary>
    /// Contains all information before a player interacts with an elevator.
    /// </summary>
    public class InteractingElevatorEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingElevatorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="elevator">
        /// <inheritdoc cref="Elevator" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingElevatorEventArgs(Player player, ElevatorChamber elevator, bool isAllowed = true)
        {
            Player = player;
            Lift = Lift.Get(elevator);
            Elevator = elevator;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Elevator" /> instance.
        /// </summary>
        public ElevatorChamber Elevator { get; }

        /// <summary>
        /// Gets the <see cref="Lift" /> instance.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the elevator.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's interacting with the elevator.
        /// </summary>
        public Player Player { get; }
    }
}