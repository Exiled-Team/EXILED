// -----------------------------------------------------------------------
// <copyright file="ChangingMoveStateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;

    using Interfaces;

    using PlayerRoles.FirstPersonControl;

    /// <summary>
    ///     Contains all information before changing movement state.
    /// </summary>
    public class ChangingMoveStateEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingMoveStateEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldState">
        ///     <inheritdoc cref="OldState" />
        /// </param>
        /// <param name="newState">
        ///     <inheritdoc cref="NewState" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingMoveStateEventArgs(Player player, PlayerMovementState oldState, PlayerMovementState newState, bool isAllowed = true)
        {
            Player = player;
            OldState = oldState;
#pragma warning disable CS0618
            NewState = newState;
#pragma warning restore CS0618
        }

        /// <summary>
        ///     Gets the player who's changing the movement state.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the old state.
        /// </summary>
        public PlayerMovementState OldState { get; }

        /// <summary>
        ///     Gets or sets the new state.
        /// </summary>
        // TODO: remove setter
        public PlayerMovementState NewState
        {
            get;
            [Obsolete("Functional was removed due desync problems")]
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the player can change the movement state.
        /// </summary>
        // TODO: remove
        [Obsolete("Functional was removed due desync problems")]
        public bool IsAllowed { get; set; } = true;
    }
}