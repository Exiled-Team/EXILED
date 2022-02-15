// -----------------------------------------------------------------------
// <copyright file="UpgradingItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems.API.EventArgs
{
    using SEXiled.API.Features;
    using SEXiled.CustomItems.API.Features;
    using SEXiled.Events.EventArgs;

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
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="knobSetting"><inheritdoc cref="Events.EventArgs.UpgradingItemEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="Events.EventArgs.UpgradingItemEventArgs.IsAllowed"/></param>
        public UpgradingItemEventArgs(Player player, ItemBase item, Scp914KnobSetting knobSetting, bool isAllowed = true)
            : base(player, item, knobSetting, isAllowed)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the upgrading <see cref="Item"/> as a<see cref="CustomItem"/>.
        /// </summary>
        public new ItemBase Item { get; }
    }
}
