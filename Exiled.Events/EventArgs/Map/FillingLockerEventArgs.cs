// -----------------------------------------------------------------------
// <copyright file="FillingLockerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    /// <summary>
    ///     Contains all information before the server spawns an item in locker.
    /// </summary>
    public class FillingLockerEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FillingLockerEventArgs" /> class.
        /// </summary>
        /// <param name="pickupBase">
        ///     <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="lockerChamber">
        ///     <inheritdoc cref="LockerChamber" />
        /// </param>
        public FillingLockerEventArgs(ItemPickupBase pickupBase, LockerChamber lockerChamber)
        {
            Pickup = Pickup.Get(pickupBase);
            LockerChamber = lockerChamber;
        }

        /// <summary>
        ///     Gets a value indicating the item being spawned.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        ///     Gets a value indicating the target locker chamber.
        /// </summary>
        public LockerChamber LockerChamber { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the item can be spawned.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}