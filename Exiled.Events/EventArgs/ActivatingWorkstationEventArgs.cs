// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player activates a workstation.
    /// </summary>
    public class ActivatingWorkstationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingWorkstationEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="station"><inheritdoc cref="WorkStation"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ActivatingWorkstationEventArgs(Player player, WorkStation station, bool isAllowed = true)
        {
            Player = player;
            Workstation = station;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's trying to activate the workstation.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a workstation.
        /// </summary>
        public WorkStation Workstation { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
