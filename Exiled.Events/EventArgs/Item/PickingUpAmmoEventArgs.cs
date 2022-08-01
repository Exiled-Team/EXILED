// -----------------------------------------------------------------------
// <copyright file="PickingUpAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces.Pickup;

    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information before a player picks up an ammo.
    /// </summary>
    public class PickingUpAmmoEventArgs : IPickupAmmoEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickingUpAmmoEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's picking up the ammo.</param>
        /// <param name="pickup">The pickup to be picked up.</param>
        /// <param name="isAllowed">Gets or sets a value indicating whether or not the ammo can be picked up.</param>
        public PickingUpAmmoEventArgs(Player player, ItemPickupBase pickup, bool isAllowed = true)
        {
            Ammo = (AmmoPickup)Pickup.Get(pickup);
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="API.Enums.AmmoType" /> of the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value representing the <see cref="AmmoPickup"/> being picked up.
        /// </summary>
        public AmmoPickup Ammo { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the ammo can be picked up.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
