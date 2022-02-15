// -----------------------------------------------------------------------
// <copyright file="InteractedEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations after a player has interacted with an interactable.
    /// </summary>
    public class InteractedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public InteractedEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the player who interacted.
        /// </summary>
        public Player Player { get; }
    }
}
