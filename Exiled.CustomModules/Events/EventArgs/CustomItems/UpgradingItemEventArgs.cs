// -----------------------------------------------------------------------
// <copyright file="UpgradingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Events.EventArgs.Scp914;

    using InventorySystem.Items;

    using Scp914;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Player"/>'s inventory item is upgraded.
    /// </summary>
    public class UpgradingItemEventArgs : UpgradingInventoryItemEventArgs, ICustomItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="item"><inheritdoc cref="UpgradingInventoryItemEventArgs.Item"/></param>
        /// <param name="knobSetting"><inheritdoc cref="UpgradingInventoryItemEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="UpgradingInventoryItemEventArgs.IsAllowed"/></param>
        public UpgradingItemEventArgs(Player player, ItemBase item, CustomItem customItem, ItemBehaviour itemBehaviour, Scp914KnobSetting knobSetting, bool isAllowed = true)
            : base(player, item, knobSetting, isAllowed)
        {
            CustomItem = customItem;
            ItemBehaviour = itemBehaviour;
        }

        /// <inheritdoc/>
        public CustomItem CustomItem { get; }

        /// <inheritdoc/>
        public ItemBehaviour ItemBehaviour { get; }
    }
}