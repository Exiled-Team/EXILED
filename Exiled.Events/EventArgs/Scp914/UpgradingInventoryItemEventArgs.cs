// -----------------------------------------------------------------------
// <copyright file="UpgradingInventoryItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using API.Features;
    using API.Features.Items;

    using global::Scp914;

    using Interfaces;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before SCP-914 upgrades an item.
    /// </summary>
    public class UpgradingInventoryItemEventArgs : IPlayerEvent, IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingInventoryItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="item">
        /// <inheritdoc cref="Item" />
        /// </param>
        /// <param name="knobSetting">
        /// <inheritdoc cref="KnobSetting" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public UpgradingInventoryItemEventArgs(Player player, ItemBase item, Scp914KnobSetting knobSetting, bool isAllowed = true)
        {
            Scp914 = API.Features.Scp914.Scp914Controller;
            Player = player;
            Item = Item.Get(item);
            KnobSetting = knobSetting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Scp914Controller" /> instance.
        /// </summary>
        public Scp914Controller Scp914 { get; }

        /// <summary>
        /// Gets or sets SCP-914 working knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the upgrade is successful.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets a list of items to be upgraded inside SCP-914.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the <see cref="Player" /> who owns the item to be upgraded.
        /// </summary>
        public Player Player { get; }
    }
}