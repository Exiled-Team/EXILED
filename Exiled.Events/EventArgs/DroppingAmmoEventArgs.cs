// -----------------------------------------------------------------------
// <copyright file="DroppingAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

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
        /// <param name="ammo"><inheritdoc cref="Ammo"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DroppingAmmoEventArgs(Player player, Ammo ammo, bool isAllowed = true)
        {
            Player = player;
            Ammo = ammo;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's dropping the ammo.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the ammo to be dropped.
        /// </summary>
        public Ammo Ammo { get; }

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
