// -----------------------------------------------------------------------
// <copyright file="ChangingNicknameEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before changing a player's in-game nickname.
    /// </summary>
    public class ChangingNicknameEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingNicknameEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's name is being changed.</param>
        /// <param name="newName">The new name to be used.</param>
        public ChangingNicknameEventArgs(Player player, string newName)
        {
            Player = player;
            OldName = player.CustomName;
            NewName = newName;
        }

        /// <summary>
        /// Gets the <see cref="Player"/>'s old name.
        /// </summary>
        public string OldName { get; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s new name.
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's name is being changed.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be allowed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}