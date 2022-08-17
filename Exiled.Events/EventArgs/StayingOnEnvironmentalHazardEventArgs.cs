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
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information when a player stays on an environmental hazard.
    /// </summary>
    public class StayingOnEnvironmentalHazardEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StayingOnEnvironmentalHazardEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="environmentalHazard"><inheritdoc cref="EnvironmentalHazard"/></param>
        public StayingOnEnvironmentalHazardEventArgs(API.Features.Player player, EnvironmentalHazard environmentalHazard)
        {
            Player = player;
            EnvironmentalHazard = environmentalHazard;
        }

        /// <summary>
        /// Gets the player who's staying on the environmental hazard.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the environmental hazard that the player is staying on.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player should be affected by the environmental hazard.
        /// </summary>
        [Obsolete("IsAllowed has been deprecated, use EnteringEnvironmentalHazardEventArgs::IsAllowed instead.", true)]
        public bool IsAllowed { get; set; }
    }
}
