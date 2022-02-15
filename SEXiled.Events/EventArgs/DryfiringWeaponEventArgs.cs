// -----------------------------------------------------------------------
// <copyright file="DryfiringWeaponEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    /// <summary>
    /// Contains all information before a player's weapon is dryfired.
    /// </summary>
    public class DryfiringWeaponEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DryfiringWeaponEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DryfiringWeaponEventArgs(Player player, bool isAllowed = true)
        {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> being dryfired.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the player who's dryfiring the weapon.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the weapon can be dryfired.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
