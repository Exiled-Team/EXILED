// -----------------------------------------------------------------------
// <copyright file="StartingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PlayerRoles.PlayableScps.Scp049;

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-049 begins recalling a player.
    /// </summary>
    public class StartingReviveEventArgs : IPlayerEvent, IDeniableEvent, IRagdollEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartingReviveEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="scp049">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="ragdoll">
        ///     <inheritdoc cref="Ragdoll" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public StartingReviveEventArgs(Player target, Player scp049, BasicRagdoll ragdoll,  Scp049ResurrectAbility.ResurrectError resurrectError,  bool isAllowed = true)
        {
            Target = target;
            Player = scp049;
            Ragdoll = Ragdoll.Get(ragdoll);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's getting recalled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the recall can begin.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the Ragdoll who's getting recalled.
        /// </summary>
        public Ragdoll Ragdoll { get; }
    }
}