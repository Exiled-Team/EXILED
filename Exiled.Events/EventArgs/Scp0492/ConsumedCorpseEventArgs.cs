// -----------------------------------------------------------------------
// <copyright file="ConsumedCorpseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp0492
{
    using System;

    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.Ragdolls;

    /// <summary>
    ///     Contains all information after zombie consumes RagDolls.
    /// </summary>
    public class ConsumedCorpseEventArgs : IScp0492Event, IRagdollEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumedCorpseEventArgs"/> class.
        /// </summary>
        /// <param name="player"> <inheritdoc cref="Player"/></param>
        /// <param name="ragDoll"> <inheritdoc cref="Ragdoll"/> </param>
        /// <remarks> See <see cref="ZombieConsumeAbility.ConsumedRagdolls"/> for all RagDolls consumed.</remarks>
        public ConsumedCorpseEventArgs(ReferenceHub player, BasicRagdoll ragDoll)
        {
            Player = Player.Get(player);
            Scp0492 = Player.Role.As<Scp0492Role>();
            Ragdoll = Ragdoll.Get(ragDoll);
        }

        /// <summary>
        ///     Gets the player who is controlling SCP-049-2.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp0492Role Scp0492 { get; }

        /// <summary>
        ///     Gets the RagDoll to be consumed.
        /// </summary>
        public Ragdoll Ragdoll { get; }

        /// <summary>
        ///     Gets or sets a value about how mush heath the Zombie will get.
        /// </summary>
        public float ConsumeHeal { get; set; } = ZombieConsumeAbility.ConsumeHeal;
    }
}
