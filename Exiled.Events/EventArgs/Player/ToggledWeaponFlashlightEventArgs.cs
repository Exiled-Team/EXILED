// -----------------------------------------------------------------------
// <copyright file="ToggledWeaponFlashlightEventArgs.cs" company="Exiled Team">
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
    /// Contains all information after a player toggles the weapon's flashlight.
    /// </summary>
    public class ToggledWeaponFlashlightEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggledWeaponFlashlightEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="newState">
        /// <inheritdoc cref="NewState" />
        /// </param>
        public ToggledWeaponFlashlightEventArgs(Player player, Firearm firearm, bool newState)
        {
            Firearm = firearm;
            Player = player;
            NewState = newState;
        }

        /// <summary>
        /// Gets a value indicating whether the new weapon's flashlight state will be enabled.
        /// </summary>
        public bool NewState { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being held.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's toggling the weapon's flashlight.
        /// </summary>
        public Player Player { get; }
    }
}