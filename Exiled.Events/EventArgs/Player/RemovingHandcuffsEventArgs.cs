// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before freeing a handcuffed player.
    /// </summary>
    public class RemovingHandcuffsEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingHandcuffsEventArgs" /> class.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        /// <param name="target">The target player to be uncuffed.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public RemovingHandcuffsEventArgs(Player cuffer, Player target, bool isAllowed = true)
        {
            Player = cuffer;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the target player to be cuffed.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be handcuffed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the cuffer player.
        /// </summary>
        public Player Player { get; }
    }
}