// -----------------------------------------------------------------------
// <copyright file="DequippedMedicalItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;

    /// <summary>
    /// Contains all informations after a player dequipes a medical item.
    /// </summary>
    public class DequippedMedicalItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DequippedMedicalItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        public DequippedMedicalItemEventArgs(Player player, ItemType item)
        {
            Player = player;
            Item = item;
        }

        /// <summary>
        /// Gets the player who used the medical item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the medical item that the player consumed.
        /// </summary>
        public ItemType Item { get; }
    }
}
