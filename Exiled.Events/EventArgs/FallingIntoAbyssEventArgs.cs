// -----------------------------------------------------------------------
// <copyright file="FallingIntoAbyssEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player falls into abyss in <see cref="RoomType.HczArmory"/>
    /// </summary>
    public class FallingIntoAbyssEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FallingIntoAbyssEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public FallingIntoAbyssEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's currently falling into abyss.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player dies by falling into abyss.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
