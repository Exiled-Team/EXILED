// -----------------------------------------------------------------------
// <copyright file="UnloadingWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    /// <summary>
    /// Contains all information before a player's weapon is unloaded.
    /// </summary>
    public class UnloadingWeaponEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadingWeaponEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UnloadingWeaponEventArgs(Player player, bool isAllowed = true) {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Items.Firearm"/> being unloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the player who's unloading the weapon.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the weapon can be unloaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
