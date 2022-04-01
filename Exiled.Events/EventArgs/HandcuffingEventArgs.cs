// -----------------------------------------------------------------------
// <copyright file="HandcuffingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before handcuffing a player.
    /// </summary>
    public class HandcuffingEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="cuffer"><inheritdoc cref="Cuffer"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public HandcuffingEventArgs(Player cuffer, Player target, bool isAllowed = true) {
            Cuffer = cuffer;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the cuffer player.
        /// </summary>
        public Player Cuffer { get; }

        /// <summary>
        /// Gets the target player to be cuffed.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be handcuffed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
