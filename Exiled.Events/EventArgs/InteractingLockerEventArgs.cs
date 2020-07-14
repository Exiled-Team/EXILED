// -----------------------------------------------------------------------
// <copyright file="InteractingLockerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player interacts with a locker.
    /// </summary>
    public class InteractingLockerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingLockerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="locker"><inheritdoc cref="Locker"/></param>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingLockerEventArgs(Player player, Locker locker, int id, bool isAllowed)
        {
            Player = player;
            Locker = locker;
            Id = id;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the locker.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the <see cref="Locker"/> instance.
        /// </summary>
        public Locker Locker { get; private set; }

        /// <summary>
        /// Gets the locker id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}