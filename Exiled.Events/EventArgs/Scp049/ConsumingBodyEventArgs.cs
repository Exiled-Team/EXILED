// -----------------------------------------------------------------------
// <copyright file="ConsumingBodyEventArgs.cs" company="Exiled Team">
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
    using PlayerRoles.PlayableScps.Scp049.Zombies;

    /// <summary>
    ///     Contains all information before zombie consumes ragdoll.
    /// </summary>
    public class ConsumingBodyEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumingBodyEventArgs"/> class.
        /// </summary>
        /// <param name="scp049"> <inheritdoc cref="ConsumingBodyEventArgs.Player"/></param>
        /// <param name="target"> <inheritdoc cref="ConsumingBodyEventArgs.TargetRagdoll"/> </param>
        /// <param name="errorCode"><inheritdoc cref="ConsumingBodyEventArgs.ErrorCode"/></param>
        /// <param name="isAllowed"><inheritdoc cref="ConsumingBodyEventArgs.IsAllowed"/></param>
        /// <remarks> See <see cref="ZombieConsumeAbility.ConsumedRagdolls"/> for all ragdolls consumed. </remarks>
        public ConsumingBodyEventArgs(Player scp049, BasicRagdoll target, byte errorCode, bool isAllowed = true)
        {
            Player = scp049;
            TargetRagdoll = target;
            ErrorCode = errorCode;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets error code to send back to client.
        /// </summary>
        public byte ErrorCode { get; set; }

        /// <summary>
        ///     Gets the Ragdoll to be consumed.
        /// </summary>
        public BasicRagdoll TargetRagdoll { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049-2.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}