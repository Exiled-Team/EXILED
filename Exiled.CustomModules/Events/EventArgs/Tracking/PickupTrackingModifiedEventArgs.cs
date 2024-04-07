// -----------------------------------------------------------------------
// <copyright file="PickupTrackingModifiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.Tracking
{
    using System.Collections.Generic;

    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Interfaces;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after modifying a pickup tracking.
    /// </summary>
    public class PickupTrackingModifiedEventArgs : TrackingModifiedEventArgs, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickupTrackingModifiedEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="previousAbilities"><inheritdoc cref="TrackingModifiedEventArgs.PreviousTrackableItems"/></param>
        /// <param name="currentAbilities"><inheritdoc cref="TrackingModifiedEventArgs.CurrentTrackableItems"/></param>
        public PickupTrackingModifiedEventArgs(Pickup pickup, IEnumerable<ITrackable> previousAbilities, IEnumerable<ITrackable> currentAbilities)
            : base(previousAbilities, currentAbilities) => Pickup = pickup;

        /// <inheritdoc/>
        public Pickup Pickup { get; }
    }
}