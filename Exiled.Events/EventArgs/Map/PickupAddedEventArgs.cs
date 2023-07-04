// -----------------------------------------------------------------------
// <copyright file="PickupAddedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information after the server spawns a pickup.
    /// </summary>
    public class PickupAddedEventArgs : IExiledEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickupAddedEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        public PickupAddedEventArgs(ItemPickupBase pickupBase)
        {
            Pickup = Pickup.Get(pickupBase);
        }

        /// <summary>
        ///     Gets a value indicating the pickup being spawned.
        /// </summary>
        public Pickup Pickup { get; }
    }
}
