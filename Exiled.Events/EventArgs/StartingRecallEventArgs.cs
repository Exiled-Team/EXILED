// -----------------------------------------------------------------------
// <copyright file="StartingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-049 begins recalling a player.
    /// </summary>
    public class StartingRecallEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingRecallEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="scp049"><inheritdoc cref="Scp049"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StartingRecallEventArgs(Player target, Player scp049, bool isAllowed = true) {
            Target = target;
            Scp049 = scp049;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's getting recalled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Scp049 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the recall can begin.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
