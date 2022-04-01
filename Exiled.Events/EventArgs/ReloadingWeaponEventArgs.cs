// -----------------------------------------------------------------------
// <copyright file="ReloadingWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    /// <summary>
    /// Contains all information before a player's weapon is reloaded.
    /// </summary>
    public class ReloadingWeaponEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadingWeaponEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReloadingWeaponEventArgs(Player player, bool isAllowed = true) {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Items.Firearm"/> being reloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the player who's reloading the weapon.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the weapon can be reloaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
