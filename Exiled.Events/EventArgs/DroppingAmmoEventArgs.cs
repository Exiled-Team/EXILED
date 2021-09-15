// -----------------------------------------------------------------------
// <copyright file="DroppingAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before a player drops an item.
    /// </summary>
    public class DroppingAmmoEventArgs : EventArgs
    {
        private bool isAllowed = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingAmmoEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="ammoType"><inheritdoc cref="AmmoType"/></param>
        /// <param name="amount"><inheritdoc cref="ushort"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DroppingAmmoEventArgs(Player player, AmmoType ammoType, ushort amount, bool isAllowed = true)
        {
            Player = player;
            AmmoType = ammoType;
            Amount = amount;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's dropping the ammo.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the type of ammo being dropped.
        /// </summary>
        public AmmoType AmmoType { get; }

        /// <summary>
        /// Gets or sets the amount of ammo being dropped.
        /// </summary>
        public ushort Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the ammo can be dropped.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                if (Player.Role == RoleType.Spectator)
                    isAllowed = true;
                return isAllowed;
            }

            set
            {
                if (Player.Role == RoleType.Spectator)
                    value = true;
                isAllowed = value;
            }
        }
    }
}
