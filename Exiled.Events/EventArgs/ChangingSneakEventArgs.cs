// -----------------------------------------------------------------------
// <copyright file="ChangingSneakEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player starts or stops sneaking.
    /// </summary>
    public class ChangingSneakEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingSneakEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isSneaking"><inheritdoc cref="IsSneaking"/></param>
        public ChangingSneakEventArgs(Player player, bool isSneaking)
        {
            Player = player;
            IsSneaking = isSneaking;
        }

        /// <summary>
        /// Gets the player who is starting/stopping sneaking.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player started or stopped sneaking.
        /// </summary>
        public bool IsSneaking { get; }
    }
}
