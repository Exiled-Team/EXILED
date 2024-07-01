// -----------------------------------------------------------------------
// <copyright file="UnloadedWeaponEventArgs.cs" company="Exiled Team">
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
    /// Contains all information after a player's weapon is unloaded.
    /// </summary>
    public class UnloadedWeaponEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadedWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        public UnloadedWeaponEventArgs(Player player, Firearm firearm)
        {
            Firearm = firearm;
            Player = player;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> being unloaded.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's unloaded the weapon.
        /// </summary>
        public Player Player { get; }
    }
}