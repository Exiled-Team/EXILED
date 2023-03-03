// -----------------------------------------------------------------------
// <copyright file="ChangingGroupEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player's user group changes.
    /// </summary>
    public class ChangingGroupEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingGroupEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newGroup">
        ///     <inheritdoc cref="NewGroup" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingGroupEventArgs(Player player, UserGroup newGroup, bool isAllowed = true)
        {
            Player = player;
            NewGroup = newGroup;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the player's new group.
        /// </summary>
        public UserGroup NewGroup { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can change groups.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's changing his group.
        /// </summary>
        public Player Player { get; }
    }
}