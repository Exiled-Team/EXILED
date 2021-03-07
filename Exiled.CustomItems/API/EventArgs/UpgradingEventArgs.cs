// -----------------------------------------------------------------------
// <copyright file="UpgradingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    using Scp914;

    /// <summary>
    /// Contains all informations of a <see cref="CustomItem"/> before a <see cref="Player"/> escapes.
    /// </summary>
    public class UpgradingEventArgs : UpgradingItemsEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        public UpgradingEventArgs(Inventory.SyncItemInfo item, UpgradingItemsEventArgs ev)
            : this(item, ev.Scp914, ev.Players, ev.Items, ev.KnobSetting, ev.IsAllowed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="scp914"><inheritdoc cref="UpgradingItemsEventArgs.Scp914"/></param>
        /// <param name="players"><inheritdoc cref="UpgradingItemsEventArgs.Players"/></param>
        /// <param name="items"><inheritdoc cref="UpgradingItemsEventArgs.Items"/></param>
        /// <param name="knobSetting"><inheritdoc cref="UpgradingItemsEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="UpgradingItemsEventArgs.IsAllowed"/></param>
        public UpgradingEventArgs(Inventory.SyncItemInfo item, Scp914Machine scp914, List<Player> players, List<Pickup> items, Scp914Knob knobSetting, bool isAllowed = true)
            : base(scp914, players, items, knobSetting, isAllowed)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the upgrading <see cref="Inventory.SyncItemInfo"/> as a<see cref="CustomItem"/>.
        /// </summary>
        public Inventory.SyncItemInfo Item { get; }
    }
}
