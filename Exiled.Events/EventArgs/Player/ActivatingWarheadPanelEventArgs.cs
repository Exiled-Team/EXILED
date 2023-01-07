// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanelEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before a player activates the warhead panel.
    /// </summary>
    public class ActivatingWarheadPanelEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivatingWarheadPanelEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ActivatingWarheadPanelEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the warhead can be activated.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's trying to activate the warhead panel.
        /// </summary>
        public Player Player { get; }
    }
}