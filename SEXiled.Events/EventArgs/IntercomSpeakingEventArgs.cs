// -----------------------------------------------------------------------
// <copyright file="IntercomSpeakingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations before a player speaks to the intercom.
    /// </summary>
    public class IntercomSpeakingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntercomSpeakingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public IntercomSpeakingEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's going to speak to the intercom.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can speak to the intercom.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
