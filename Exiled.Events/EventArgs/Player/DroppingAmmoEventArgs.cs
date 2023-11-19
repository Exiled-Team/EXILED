// -----------------------------------------------------------------------
// <copyright file="DroppingAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Enums;
    using API.Features;

    using Interfaces;

    using PlayerRoles;

    /// <summary>
    ///     Contains all information before a player drops ammo.
    /// </summary>
    public class DroppingAmmoEventArgs : IPlayerEvent, IDeniableEvent
    {
        private bool isAllowed = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DroppingAmmoEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="ammoType">
        ///     <inheritdoc cref="AmmoType" />
        /// </param>
        /// <param name="amount">
        ///     <inheritdoc cref="int" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DroppingAmmoEventArgs(Player player, AmmoType ammoType, ushort amount, bool isAllowed = true)
        {
            Player = player;
            AmmoType = ammoType;
            Amount = amount;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the type of ammo being dropped.
        /// </summary>
        public AmmoType AmmoType { get; }

        /// <summary>
        ///     Gets or sets the amount of ammo being dropped.
        /// </summary>
        public ushort Amount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the ammo can be dropped.
        /// </summary>
        public bool IsAllowed
        {
            get
            {
                if (Player.Role == RoleTypeId.Spectator)
                    isAllowed = true;
                return isAllowed;
            }

            set
            {
                if (Player.Role == RoleTypeId.Spectator)
                    value = true;
                isAllowed = value;
            }
        }

        /// <summary>
        ///     Gets the player who's dropping the ammo.
        /// </summary>
        public Player Player { get; }
    }
}
