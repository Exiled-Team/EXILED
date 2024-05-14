// -----------------------------------------------------------------------
// <copyright file="UpgradingPickupEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using System;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Scp914Processors;
    using Exiled.Events.EventArgs.Interfaces;
    using global::Scp914;
    using InventorySystem.Items.Pickups;
    using UnityEngine;

    /// <summary>
    /// Contains all information before SCP-914 upgrades an item.
    /// </summary>
    public class UpgradingPickupEventArgs : IPickupEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingPickupEventArgs" /> class.
        /// </summary>
        /// <param name="item">
        /// <inheritdoc cref="Pickup" />
        /// </param>
        /// <param name="newPos">
        /// <inheritdoc cref="OutputPosition" />
        /// </param>
        /// <param name="knobSetting">
        /// <inheritdoc cref="KnobSetting" />
        /// </param>
        /// <param name="processor">
        /// <inheritdoc cref="Processor"/>
        /// </param>
        public UpgradingPickupEventArgs(ItemPickupBase item, Vector3 newPos, Scp914KnobSetting knobSetting, Scp914Processor processor)
        {
            Pickup = Pickup.Get(item);
            OutputPosition = newPos;
            KnobSetting = knobSetting;
            Processor = processor;
        }

        /// <summary>
        /// Gets a list of items to be upgraded inside SCP-914.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets or sets the position the item will be output to.
        /// </summary>
        public Vector3 OutputPosition { get; set; }

        /// <summary>
        /// Gets or sets SCP-914 working knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; set; }

        /// <summary>
        /// Gets the <see cref="Scp914Processor"/> for this item.
        /// </summary>
        public Scp914Processor Processor { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the upgrade is successful.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}