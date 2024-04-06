// -----------------------------------------------------------------------
// <copyright file="ElevatorArrivedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after an elevator has arrived.
    /// </summary>
    public class ElevatorArrivedEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorArrivedEventArgs"/> class.
        /// </summary>
        /// <param name="lift"><inheritdoc cref="Lift"/></param>
        public ElevatorArrivedEventArgs(Lift lift)
        {
            Lift = lift;
            Players = Player.Get(x => x.Lift == Lift).ToList();
        }

        /// <summary>
        /// Gets an elevator.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets the players in the elevator.
        /// </summary>
        public IReadOnlyCollection<Player> Players { get; }
    }
}