// -----------------------------------------------------------------------
// <copyright file="PickingUpArmorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before a player picks up <see cref="API.Features.Items.Armor"/>.
    /// </summary>
    public class PickingUpArmorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpArmorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="PickingUpItemEventArgs.Player"/></param>
        /// <param name="pickup"><inheritdoc cref="PickingUpItemEventArgs.Pickup"/></param>
        /// <param name="isAllowed"><inheritdoc cref="PickingUpItemEventArgs.IsAllowed"/></param>
        public PickingUpArmorEventArgs(Player player, ItemPickupBase pickup, bool isAllowed = true)
        {
            Armor = (BodyArmorPickup)Pickup.Get(pickup);
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who dropped the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the dropped BodyArmor.
        /// </summary>
        public BodyArmorPickup Armor { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the BodyArmor can be picked up.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
