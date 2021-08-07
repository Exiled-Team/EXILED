// -----------------------------------------------------------------------
// <copyright file="ThrowingGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player throws a greande.
    /// </summary>
    public class ThrowingGrenadeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <param name="isSlow"><inheritdoc cref="IsSlow"/></param>
        /// <param name="fuseTime"><inheritdoc cref="FuseTime"/></param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public ThrowingGrenadeEventArgs(Player player, GrenadeType type, bool isSlow, double fuseTime, bool isAllowed = true)
        {
            Player = player;
            Type = type;
            IsSlow = isSlow;
            FuseTime = fuseTime;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's throwing the greande.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the grenade type.
        /// </summary>
        public GrenadeType Type { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the throw is slow or not.
        /// </summary>
        public bool IsSlow { get; set; }

        /// <summary>
        /// Gets or sets the fuse time.
        /// </summary>
        public double FuseTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
