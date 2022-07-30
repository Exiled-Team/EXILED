// -----------------------------------------------------------------------
// <copyright file="IPickupArmorEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Pickup
{
    using Exiled.API.Features.Pickups;

    /// <summary>
    ///     Event args used for all <see cref="BodyArmorPickup" /> related events.
    /// </summary>
    public interface IPickupArmorEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="BodyArmorPickup" /> triggering the event.
        /// </summary>
        public BodyArmorPickup Armor { get; }
    }
}
