// -----------------------------------------------------------------------
// <copyright file="DroppingNullEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player drops a null item.
    /// </summary>
    public class DroppingNullEventArgs : EventArgs // TODO: Rename to DroppingNothingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroppingNullEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public DroppingNullEventArgs(Player player) => Player = player;

        /// <summary>
        /// Gets the player who's dropping the null item.
        /// </summary>
        public Player Player { get; }
    }
}
