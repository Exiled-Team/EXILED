// -----------------------------------------------------------------------
// <copyright file="IPickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Pickups;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Pickups.Pickup" /> related events.
    /// </summary>
    public interface IPickupEvent : IExiledEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Pickups.Pickup" /> triggering the event.
        /// </summary>
        public Pickup Pickup { get; }
    }
}