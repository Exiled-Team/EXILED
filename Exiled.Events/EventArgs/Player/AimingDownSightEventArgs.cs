// -----------------------------------------------------------------------
// <copyright file="AimingDownSightEventArgs.cs" company="Exiled Team">
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
    /// Contains all information when a player aims.
    /// </summary>
    public class AimingDownSightEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AimingDownSightEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        /// <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="aiming">
        /// <inheritdoc cref="Aiming" />
        /// </param>
        public AimingDownSightEventArgs(Player player, Firearm firearm, bool aiming)
        {
            Firearm = firearm;
            Player = player;
            Aiming = aiming;
        }

        /// <summary>
        /// Gets a value indicating whether the player starts aiming or stops aiming.
        /// </summary>
        public bool Aiming { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm" /> used to trigger the aim action.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the player who's triggering the aim action.
        /// </summary>
        public Player Player { get; }
    }
}