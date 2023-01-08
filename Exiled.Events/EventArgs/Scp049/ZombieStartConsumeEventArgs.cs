// -----------------------------------------------------------------------
// <copyright file="ZombieStartConsumeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using System.Collections.Generic;

    using API.Features;
    using Interfaces;
    using Mirror;

    /// <summary>
    ///     Contains all information before zombie consumes ragdoll.
    /// </summary>
    public class ZombieStartConsumeEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZombieStartConsumeEventArgs"/> class.
        /// </summary>
        /// <param name="scp049"> <inheritdoc cref="ZombieStartConsumeEventArgs.Player"/></param>
        /// <param name="target"> <inheritdoc cref="ZombieStartConsumeEventArgs.TargetRagdoll"/> </param>
        /// <param name="consumedRagdolls"> <inheritdoc cref="ZombieStartConsumeEventArgs.ConsomedRagdolls"/> </param>
        /// <param name="errorCode"><inheritdoc cref="ZombieStartConsumeEventArgs.ErrorCode"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ZombieStartConsumeEventArgs.IsAllowed"/></param>
        public ZombieStartConsumeEventArgs(Player scp049, BasicRagdoll target, HashSet<BasicRagdoll> consumedRagdolls, byte errorCode, bool isAllowed = true)
        {
            Player = scp049;
            TargetRagdoll = target;
            ConsomedRagdolls = consumedRagdolls;
            ErrorCode = errorCode;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets error code to send back to client.
        /// </summary>
        public byte ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets scp0492 ragdolls consumed thus far.
        /// </summary>
        public HashSet<BasicRagdoll> ConsomedRagdolls { get; set; }

        /// <summary>
        ///     Gets the Ragdoll to be consumed.
        /// </summary>
        public BasicRagdoll TargetRagdoll { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}