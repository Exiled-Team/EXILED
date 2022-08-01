// -----------------------------------------------------------------------
// <copyright file="AddingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using UnityEngine;

    /// <summary>
    /// Contains all information before adding the a player in player list.
    /// </summary>
    public class AddingPlayerEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to be added to list.</param>
        /// <param name="maxPlayers">The maximum number of players.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public AddingPlayerEventArgs(GameObject gameObject, int maxPlayers, bool isAllowed = true)
        {
            GameObject = gameObject;
            MaxPlayers = maxPlayers;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the player.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets the maximum number of players.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player will be added in Player list.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
