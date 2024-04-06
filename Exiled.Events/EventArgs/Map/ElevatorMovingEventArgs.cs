// -----------------------------------------------------------------------
// <copyright file="ElevatorMovingEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before elevator starts moving.
    /// </summary>
    public class ElevatorMovingEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorMovingEventArgs"/> class.
        /// </summary>
        /// <param name="lift"><inheritdoc cref="Lift"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ElevatorMovingEventArgs(Lift lift, bool isAllowed = true)
        {
            Lift = lift;
            Players = Player.Get(x => x.Lift == lift).ToList();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the current elevator.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Gets a list of all players in elevator.
        /// </summary>
        public IReadOnlyCollection<Player> Players { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}