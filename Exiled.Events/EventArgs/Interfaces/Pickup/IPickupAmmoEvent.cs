// -----------------------------------------------------------------------
// <copyright file="IPickupAmmoEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Pickup
{
    using Exiled.API.Features.Pickups;

    /// <summary>
    ///     Event args used for all <see cref="AmmoPickup" /> related events.
    /// </summary>
    public interface IPickupAmmoEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="AmmoPickup" /> triggering the event.
        /// </summary>
        public AmmoPickup Ammo { get; }
    }
}
