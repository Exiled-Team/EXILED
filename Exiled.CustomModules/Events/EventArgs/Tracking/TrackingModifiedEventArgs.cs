// -----------------------------------------------------------------------
// <copyright file="TrackingModifiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.Tracking
{
    using System.Collections.Generic;

    using Exiled.CustomModules.API.Interfaces;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after modifying a tracking.
    /// </summary>
    public class TrackingModifiedEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingModifiedEventArgs"/> class.
        /// </summary>
        /// <param name="previousAbilities"><inheritdoc cref="PreviousTrackableItems"/></param>
        /// <param name="currentAbilities"><inheritdoc cref="CurrentTrackableItems"/></param>
        public TrackingModifiedEventArgs(IEnumerable<ITrackable> previousAbilities, IEnumerable<ITrackable> currentAbilities)
        {
            PreviousTrackableItems = previousAbilities;
            CurrentTrackableItems = currentAbilities;
        }

        /// <summary>
        /// Gets all previous abilities.
        /// </summary>
        public IEnumerable<ITrackable> PreviousTrackableItems { get; }

        /// <summary>
        /// Gets all current abilities.
        /// </summary>
        public IEnumerable<ITrackable> CurrentTrackableItems { get; }
    }
}