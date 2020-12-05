// -----------------------------------------------------------------------
// <copyright file="ReloadingWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player's weapon is reloaded.
    /// </summary>
    public class ReloadingWeaponEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadingWeaponEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAnimationOnly"><inheritdoc cref="IsAnimationOnly"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReloadingWeaponEventArgs(Player player, bool isAnimationOnly, bool isAllowed = true)
        {
            Player = player;
            IsAnimationOnly = isAnimationOnly;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's reloading the weapon.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether only the reload animation is being reproduced or not.
        /// </summary>
        public bool IsAnimationOnly { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the weapon can be reloaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
