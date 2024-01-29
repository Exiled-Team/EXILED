// -----------------------------------------------------------------------
// <copyright file="UpgradingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Scp914;

    using Scp914;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a <see cref="API.Features.CustomItems.CustomItem"/> gets upgraded.
    /// </summary>
    public class UpgradingEventArgs : UpgradingPickupEventArgs, ICustomPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="UpgradingPickupEventArgs.Pickup"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="newPos"><inheritdoc cref="UpgradingPickupEventArgs.OutputPosition"/></param>
        /// <param name="knobSetting"><inheritdoc cref="UpgradingPickupEventArgs.KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="UpgradingPickupEventArgs.IsAllowed"/></param>
        public UpgradingEventArgs(Pickup pickup, CustomItem customItem, ItemBehaviour itemBehaviour, Vector3 newPos, Scp914KnobSetting knobSetting, bool isAllowed = true)
            : base(pickup.Base, newPos, knobSetting)
        {
            IsAllowed = isAllowed;
            CustomItem = customItem;
            ItemBehaviour = itemBehaviour;
        }

        /// <inheritdoc/>
        public CustomItem CustomItem { get; }

        /// <inheritdoc/>
        public ItemBehaviour ItemBehaviour { get; }
    }
}