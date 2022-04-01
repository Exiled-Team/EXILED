// -----------------------------------------------------------------------
// <copyright file="UpgradingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features.Items;

    using global::Scp914;

    using InventorySystem.Items.Pickups;

    using UnityEngine;

    /// <summary>
    /// Contains all information before SCP-914 upgrades an item.
    /// </summary>
    public class UpgradingItemEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingItemEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="ItemPickupBase"/></param>
        /// <param name="newPos"><inheritdoc cref="OutputPosition"/></param>
        /// <param name="knobSetting"><inheritdoc cref="KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UpgradingItemEventArgs(ItemPickupBase item, Vector3 newPos, Scp914KnobSetting knobSetting, bool isAllowed = true) {
            Scp914 = API.Features.Scp914.Scp914Controller;
            Item = Pickup.Get(item);
            OutputPosition = newPos;
            KnobSetting = knobSetting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Scp914Controller"/> instance.
        /// </summary>
        public Scp914Controller Scp914 { get; }

        /// <summary>
        /// Gets or sets the position the item will be output to.
        /// </summary>
        public Vector3 OutputPosition { get; set; }

        /// <summary>
        /// Gets a list of items to be upgraded inside SCP-914.
        /// </summary>
        public Pickup Item { get; }

        /// <summary>
        /// Gets or sets SCP-914 working knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the upgrade is successful.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
