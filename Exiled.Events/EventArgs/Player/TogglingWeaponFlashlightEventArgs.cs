// -----------------------------------------------------------------------
// <copyright file="TogglingWeaponFlashlightEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before a player toggles the weapon's flashlight.
    /// </summary>
    public class TogglingWeaponFlashlightEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TogglingWeaponFlashlightEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newState">
        ///     <inheritdoc cref="NewState" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public TogglingWeaponFlashlightEventArgs(Player player, bool newState, bool isAllowed = true)
        {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the new weapon's flashlight state.
        /// </summary>
        public bool NewState { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Firearm Firearm { get; }

        /// <inheritdoc />
        public Item Item => Firearm;

        /// <inheritdoc />
        public Player Player { get; }
    }
}