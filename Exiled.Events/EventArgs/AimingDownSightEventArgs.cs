// -----------------------------------------------------------------------
// <copyright file="AimingDownSightEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

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
        public AimingDownSightEventArgs(Player player, bool adsIn)
        {
            if (player?.CurrentItem is Firearm firearm)
                Firearm = firearm;
            else
                Firearm = null;

            Player = player;
            AdsIn = adsIn;
            AdsOut = !adsIn;
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
