// -----------------------------------------------------------------------
// <copyright file="PickingUpAmmoEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Enums;
    using SEXiled.API.Extensions;
    using SEXiled.API.Features;

    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before a player picks up an ammo.
    /// </summary>
    public class PickingUpAmmoEventArgs : PickingUpItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickingUpAmmoEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's picking up the ammo.</param>
        /// <param name="pickup">The pickup to be picked up.</param>
        /// <param name="isAllowed">Gets or sets a value indicating whether or not the ammo can be picked up.</param>
        public PickingUpAmmoEventArgs(Player player, ItemPickupBase pickup, bool isAllowed = true)
            : base(player, pickup, isAllowed)
        {
        }

        /// <summary>
        /// Gets the <see cref="API.Enums.AmmoType"/> of the item.
        /// </summary>
        public AmmoType AmmoType => Pickup.Type.GetAmmoType();
    }
}
