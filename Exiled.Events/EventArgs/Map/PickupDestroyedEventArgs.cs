// -----------------------------------------------------------------------
// <copyright file="PickupDestroyedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information after the server destroy a pickup.
    /// </summary>
    public class PickupDestroyedEventArgs : IExiledEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickupDestroyedEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        public PickupDestroyedEventArgs(ItemPickupBase pickupBase)
        {
            Pickup = Pickup.Get(pickupBase);
        }

        /// <summary>
        ///     Gets a value indicating the pickup being destroyed.
        /// </summary>
        public Pickup Pickup { get; }
    }
}
