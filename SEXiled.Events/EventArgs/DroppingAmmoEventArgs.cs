// -----------------------------------------------------------------------
// <copyright file="DroppingAmmoEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Enums;
    using SEXiled.API.Features;

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
        /// <param name="amount"><inheritdoc cref="int"/></param>
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
        public int Amount { get; set; }

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
