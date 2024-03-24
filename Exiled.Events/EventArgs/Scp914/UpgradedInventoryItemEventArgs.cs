// -----------------------------------------------------------------------
// <copyright file="UpgradedInventoryItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using global::Scp914;

    /// <summary>
    /// Contains all information after SCP-914 upgrades an item.
    /// </summary>
    public class UpgradedInventoryItemEventArgs : IItemEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradedInventoryItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="scp914KnobSetting"><inheritdoc cref="Setting"/></param>
        public UpgradedInventoryItemEventArgs(Player player, Item item, Scp914KnobSetting scp914KnobSetting)
        {
            Player = player;
            Item = item;
            Setting = scp914KnobSetting;
        }

        /// <summary>
        /// Gets the <see cref="Scp914KnobSetting"/> on which item was upgraded.
        /// </summary>
        public Scp914KnobSetting Setting { get; }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}