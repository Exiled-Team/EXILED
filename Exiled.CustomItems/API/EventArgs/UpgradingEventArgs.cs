// -----------------------------------------------------------------------
// <copyright file="UpgradingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs {
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;

    using InventorySystem.Items.Pickups;

    using Scp914;

    using UnityEngine;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Item"/> gets upgraded.
    /// </summary>
    public class UpgradingEventArgs : Exiled.Events.EventArgs.UpgradingItemEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="newPos"><inheritdoc cref="Events.EventArgs.UpgradingItemEventArgs.OutputPosition"/></param>
        /// <param name="knobSetting"><inheritdoc cref="Events.EventArgs.UpgradingItemEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="Events.EventArgs.UpgradingItemEventArgs.IsAllowed"/></param>
        public UpgradingEventArgs(ItemPickupBase item, Vector3 newPos, Scp914KnobSetting knobSetting, bool isAllowed = true)
            : base(item, newPos, knobSetting, isAllowed) {
            Item = item;
        }

        /// <summary>
        /// Gets the upgrading <see cref="Item"/> as a<see cref="CustomItem"/>.
        /// </summary>
        public new ItemPickupBase Item { get; }
    }
}
