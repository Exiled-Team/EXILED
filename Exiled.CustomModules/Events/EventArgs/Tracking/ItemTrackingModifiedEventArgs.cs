// -----------------------------------------------------------------------
// <copyright file="ItemTrackingModifiedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.Tracking
{
    using System.Collections.Generic;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all informations after modifying an item tracking.
    /// </summary>
    public class ItemTrackingModifiedEventArgs : TrackingModifiedEventArgs, IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTrackingModifiedEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Pickup"/></param>
        /// <param name="previousAbilities"><inheritdoc cref="TrackingModifiedEventArgs.PreviousAbilities"/></param>
        /// <param name="currentAbilities"><inheritdoc cref="TrackingModifiedEventArgs.CurrentAbilities"/></param>
        public ItemTrackingModifiedEventArgs(Item item, IEnumerable<IAbilityBehaviour> previousAbilities, IEnumerable<IAbilityBehaviour> currentAbilities)
            : base(previousAbilities, currentAbilities) => Item = item;

        /// <inheritdoc/>
        public Item Item { get; }
    }
}