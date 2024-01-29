// -----------------------------------------------------------------------
// <copyright file="UsingRadioPickupBatteryEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before radio pickup battery drains.
    /// </summary>
    public class UsingRadioPickupBatteryEventArgs : IDeniableEvent, IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingRadioPickupBatteryEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="RadioPickup"/></param>
        /// <param name="drain"><inheritdoc cref="Drain"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingRadioPickupBatteryEventArgs(InventorySystem.Items.Radio.RadioPickup pickup, float drain, bool isAllowed = true)
        {
            RadioPickup = Pickup.Get(pickup).As<RadioPickup>();
            Drain = drain;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Pickup Pickup => RadioPickup;

        /// <inheritdoc cref="Pickup"/>
        public RadioPickup RadioPickup { get; }

        /// <summary>
        /// Gets or sets the radio percent drain.
        /// </summary>
        public float Drain { get; set; }
    }
}