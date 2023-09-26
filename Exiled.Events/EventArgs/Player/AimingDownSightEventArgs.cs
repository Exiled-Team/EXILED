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
    ///     Contains all information when a player aims.
    /// </summary>
    // TODO: remove stupid AdsIn/AdsOut propetry, and let exists only one
    public class AimingDownSightEventArgs : IPlayerEvent, IFirearmEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AimingDownSightEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        ///     <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="adsIn">
        ///     <inheritdoc cref="AdsIn" />
        /// </param>
        /// <param name="adsOut">
        ///     <inheritdoc cref="AdsOut" />
        /// </param>
        public AimingDownSightEventArgs(Player player, Firearm firearm, bool adsIn, bool adsOut)
        {
            Firearm = firearm;
            Player = player;
            AdsIn = adsIn;
            AdsOut = adsOut;
        }

        /// <summary>
        ///     Gets a value indicating whether or not the player is aiming down sight in.
        /// </summary>
        public bool AdsIn { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the player is aiming down sight out.
        /// </summary>
        public bool AdsOut { get; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Firearm" /> used to trigger the aim action.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        ///     Gets the player who's triggering the aim action.
        /// </summary>
        public Player Player { get; }
    }
}