// -----------------------------------------------------------------------
// <copyright file="TrackingModifiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.Tracking
{
    using System.Collections.Generic;

    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all informations after modifying a tracking.
    /// </summary>
    public class TrackingModifiedEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingModifiedEventArgs"/> class.
        /// </summary>
        /// <param name="previousAbilities"><inheritdoc cref="PreviousAbilities"/></param>
        /// <param name="currentAbilities"><inheritdoc cref="CurrentAbilities"/></param>
        public TrackingModifiedEventArgs(IEnumerable<IAbilityBehaviour> previousAbilities, IEnumerable<IAbilityBehaviour> currentAbilities)
        {
            PreviousAbilities = previousAbilities;
            CurrentAbilities = currentAbilities;
        }

        /// <summary>
        /// Gets all previous abilities.
        /// </summary>
        public IEnumerable<IAbilityBehaviour> PreviousAbilities { get; }

        /// <summary>
        /// Gets all current abilities.
        /// </summary>
        public IEnumerable<IAbilityBehaviour> CurrentAbilities { get; }
    }
}