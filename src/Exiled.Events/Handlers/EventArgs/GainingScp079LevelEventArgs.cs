// -----------------------------------------------------------------------
// <copyright file="GainingScp079LevelEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-079 gains a level.
    /// </summary>
    public class GainingScp079LevelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GainingScp079LevelEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="oldLevel"><inheritdoc cref="OldLevel"/></param>
        /// <param name="newLevel"><inheritdoc cref="NewLevel"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public GainingScp079LevelEventArgs(Player player, int oldLevel, int newLevel, bool isAllowed = true)
        {
            Player = player;
            OldLevel = oldLevel;
            NewLevel = newLevel;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the old level of SCP-079.
        /// </summary>
        public int OldLevel { get; private set; }

        /// <summary>
        /// Gets or sets the new level of SCP-079.
        /// </summary>
        public int NewLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}