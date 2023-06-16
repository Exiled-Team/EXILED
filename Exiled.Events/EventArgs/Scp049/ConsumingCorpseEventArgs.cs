// -----------------------------------------------------------------------
// <copyright file="ConsumingCorpseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp049.Zombies;

    /// <summary>
    ///     Contains all information before zombie consumes RagDolls.
    /// </summary>
    public class ConsumingCorpseEventArgs : IPlayerEvent, IRagdollEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumingCorpseEventArgs"/> class.
        /// </summary>
        /// <param name="player"> <inheritdoc cref="Player"/></param>
        /// <param name="ragDoll"> <inheritdoc cref="Ragdoll"/> </param>
        /// <param name="error"> <inheritdoc cref="ErrorCode"/> </param>
        /// <param name="isAllowed"> <inheritdoc cref="IsAllowed"/> </param>
        /// <remarks> See <see cref="ZombieConsumeAbility.ConsumedRagdolls"/> for all RagDolls consumed. </remarks>
        public ConsumingCorpseEventArgs(Player player, Ragdoll ragDoll, ZombieConsumeAbility.ConsumeError error, bool isAllowed = true)
        {
            Player = player;
            Ragdoll = ragDoll;
            ErrorCode = error;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Ragdoll Ragdoll { get; }

        /// <summary>
        /// Gets or sets error code to send back to client.
        /// </summary>
        public ZombieConsumeAbility.ConsumeError ErrorCode { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}
