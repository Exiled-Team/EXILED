// -----------------------------------------------------------------------
// <copyright file="UpgradingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Scp914;

    using InventorySystem.Items;

    using Scp914;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Player"/>'s inventory item is upgraded.
    /// </summary>
    public class UpgradingItemEventArgs : UpgradingInventoryItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="UpgradingInventoryItemEventArgs.Item"/></param>
        /// <param name="knobSetting"><inheritdoc cref="UpgradingInventoryItemEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="UpgradingInventoryItemEventArgs.IsAllowed"/></param>
        public UpgradingItemEventArgs(Player player, ItemBase item, Scp914KnobSetting knobSetting, bool isAllowed = true)
            : base(player, item, knobSetting, isAllowed)
        {
        }
    }
}