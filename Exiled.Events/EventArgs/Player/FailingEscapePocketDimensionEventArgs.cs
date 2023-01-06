// -----------------------------------------------------------------------
// <copyright file="FailingEscapePocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player dies from walking through an incorrect exit in the pocket dimension.
    /// </summary>
    public class FailingEscapePocketDimensionEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FailingEscapePocketDimensionEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="teleporter">
        ///     <inheritdoc cref="Teleporter" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public FailingEscapePocketDimensionEventArgs(Player player, PocketDimensionTeleport teleporter, bool isAllowed = true)
        {
            Player = player;
            Teleporter = teleporter;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the PocketDimensionTeleport the player walked into.
        /// </summary>
        public PocketDimensionTeleport Teleporter { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player dies by failing the pocket dimension escape.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's escaping the pocket dimension.
        /// </summary>
        public Player Player { get; }
    }
}