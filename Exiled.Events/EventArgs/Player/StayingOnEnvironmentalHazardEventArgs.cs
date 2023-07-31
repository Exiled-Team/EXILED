// -----------------------------------------------------------------------
// <copyright file="StayingOnEnvironmentalHazardEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using Exiled.API.Features.Hazards;
    using Hazards;
    using Interfaces;

    /// <summary>
    /// Contains all information when a player stays on an environmental hazard.
    /// </summary>
    public class StayingOnEnvironmentalHazardEventArgs : IPlayerEvent, IHazardEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StayingOnEnvironmentalHazardEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="environmentalHazard"><inheritdoc cref="EnvironmentalHazard"/></param>
        public StayingOnEnvironmentalHazardEventArgs(API.Features.Player player, EnvironmentalHazard environmentalHazard)
        {
            Player = player;
            Hazard = Hazard.Get(environmentalHazard);
        }

        /// <summary>
        /// Gets the player who's staying on the environmental hazard.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the environmental hazard that the player is staying on.
        /// </summary>
        [Obsolete("Use Hazard property instead.")]
        public EnvironmentalHazard EnvironmentalHazard => Hazard.Base;

        /// <inheritdoc cref="EnvironmentalHazard"/>
        public Hazard Hazard { get; }
    }
}