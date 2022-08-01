// -----------------------------------------------------------------------
// <copyright file="RemovingPlayerEventArgs.cs" company="Exiled Team">
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
    /// <remarks>If your server has a idle mode, don't forget to set this mode if there are no players.</remarks>
    public class RemovingPlayerEventArgs : AddingPlayerEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to be added to list.</param>
        /// <param name="maxPlayers">The maximum number of players.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public RemovingPlayerEventArgs(GameObject gameObject, int maxPlayers, bool isAllowed = true)
            : base(gameObject, maxPlayers, isAllowed)
        {
        }
    }
}
