// -----------------------------------------------------------------------
// <copyright file="FinishingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before SCP-049 finishes recalling a player.
    /// </summary>
    public class FinishingRecallEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishingRecallEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="scp049"><inheritdoc cref="Scp049"/></param>
        /// <param name="ragdoll"><inheritdoc cref="Ragdoll"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public FinishingRecallEventArgs(Player target, Player scp049, global::Ragdoll ragdoll, bool isAllowed = true)
        {
            Target = target;
            Scp049 = scp049;
            Ragdoll = Ragdoll.Get(ragdoll);
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
        /// Gets the Ragdoll who's getting recalled.
        /// </summary>
        public Ragdoll Ragdoll { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be recalled.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
