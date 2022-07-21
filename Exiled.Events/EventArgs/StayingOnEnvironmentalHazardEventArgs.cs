// -----------------------------------------------------------------------
// <copyright file="StayingOnEnvironmentalHazardEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information when a player stays on an environmental hazard.
    /// </summary>
    public class StayingOnEnvironmentalHazardEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StayingOnEnvironmentalHazardEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="environmentalHazard"><inheritdoc cref="EnvironmentalHazard"/></param>
        public StayingOnEnvironmentalHazardEventArgs(Player player, EnvironmentalHazard environmentalHazard)
        {
            Player = player;
            EnvironmentalHazard = environmentalHazard;
        }

        /// <summary>
        /// Gets the player who's staying on the environmental hazard.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the environmental hazard that the player is staying on.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }
    }
}
