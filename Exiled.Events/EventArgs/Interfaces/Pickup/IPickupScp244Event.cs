// -----------------------------------------------------------------------
// <copyright file="IPickupScp244Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Pickup
{
    using Exiled.API.Features.Pickups;

    /// <summary>
    ///     Event args used for all <see cref="Scp244Pickup" /> related events.
    /// </summary>
    public interface IPickupScp244Event : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="Scp244Pickup" /> triggering the event.
        /// </summary>
        public Scp244Pickup Scp244 { get; }
    }
}
