// -----------------------------------------------------------------------
// <copyright file="DryfiringWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player's weapon is dryfired.
    /// </summary>
    public class DryfiringWeaponEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DryfiringWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DryfiringWeaponEventArgs(Player player, bool isAllowed = true)
        {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the weapon can be dryfired.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Firearm" /> being dryfired.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        ///     Gets the player who's dryfiring the weapon.
        /// </summary>
        public Player Player { get; }
    }
}
