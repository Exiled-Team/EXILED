// -----------------------------------------------------------------------
// <copyright file="AimingDownSightEventArgs.cs" company="SEXiled Team">
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
    /// Contains all information when a player aims.
    /// </summary>
    public class AimingDownSightEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AimingDownSightEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="adsIn"><inheritdoc cref="AdsIn"/></param>
        /// <param name="adsOut"><inheritdoc cref="AdsOut"/></param>
        public AimingDownSightEventArgs(Player player, bool adsIn, bool adsOut)
        {
            Firearm = player.CurrentItem as Firearm;
            Player = player;
            AdsIn = adsIn;
            AdsOut = adsOut;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> used to trigger the aim action.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the player who's triggering the aim action.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player is aiming down sight in.
        /// </summary>
        public bool AdsIn { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player is aiming down sight out.
        /// </summary>
        public bool AdsOut { get; }
    }
}
