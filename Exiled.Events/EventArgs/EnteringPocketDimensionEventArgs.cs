// -----------------------------------------------------------------------
// <copyright file="EnteringPocketDimensionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player enters the pocket dimension.
    /// </summary>
    public class EnteringPocketDimensionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnteringPocketDimensionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="scp106"><inheritdoc cref="Scp106"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EnteringPocketDimensionEventArgs(Player player, Vector3 position, Player scp106, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            Scp106 = scp106;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's entering the pocket dimension.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the SCP-106 who sent the player to the pocket dimension.
        /// </summary>
        public Player Scp106 { get; }

        /// <summary>
        /// Gets or sets the pocket dimension position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can enter the pocket dimension.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
