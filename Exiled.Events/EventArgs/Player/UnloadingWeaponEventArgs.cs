// -----------------------------------------------------------------------
// <copyright file="UnloadingWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player's weapon is unloaded.
    /// </summary>
    public class UnloadingWeaponEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadingWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public UnloadingWeaponEventArgs(Player player, Firearm firearm, bool isAllowed = true)
        {
            Firearm = firearm;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the weapon can be unloaded.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being unloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's unloading the weapon.
        /// </summary>
        public Player Player { get; }
    }
}