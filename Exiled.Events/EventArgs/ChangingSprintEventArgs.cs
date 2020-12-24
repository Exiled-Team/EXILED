// -----------------------------------------------------------------------
// <copyright file="ChangingSprintEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player starts or stops sprinting.
    /// </summary>
    public class ChangingSprintEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingSprintEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isSprinting"><inheritdoc cref="IsSprinting"/></param>
        public ChangingSprintEventArgs(Player player, bool isSprinting)
        {
            Player = player;
            IsSprinting = isSprinting;
        }

        /// <summary>
        /// Gets the player who is starting/stopping sprinting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player started or stopped sprinting.
        /// </summary>
        public bool IsSprinting { get; }
    }
}
