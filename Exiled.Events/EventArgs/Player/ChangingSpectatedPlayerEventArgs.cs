// -----------------------------------------------------------------------
// <copyright file="ChangingSpectatedPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a spectator changes the spectated player.
    /// </summary>
    public class ChangingSpectatedPlayerEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingSpectatedPlayerEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldTarget">
        ///     <inheritdoc cref="OldTarget" />
        /// </param>
        /// <param name="newTarget">
        ///     <inheritdoc cref="NewTarget" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingSpectatedPlayerEventArgs(Player player, Player oldTarget, Player newTarget, bool isAllowed)
        {
            Player = player;
            OldTarget = oldTarget;
            NewTarget = newTarget;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets player that was spectated.
        /// </summary>
        public Player OldTarget { get; }

        /// <summary>
        ///     Gets or sets the player who's going to be spectated.
        /// </summary>
        public Player NewTarget { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not spectated player value can be activated.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets player that is changing spectated player.
        /// </summary>
        public Player Player { get; }
    }
}