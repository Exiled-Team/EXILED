// -----------------------------------------------------------------------
// <copyright file="StartingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-049 begins recalling a player.
    /// </summary>
    public class StartingRecallEventArgs : IPlayerEvent, IRagdollEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartingRecallEventArgs" /> class.
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
        public StartingRecallEventArgs(Player target, Player scp049, Ragdoll ragdoll, bool isAllowed = true)
        {
            Target = target;
            Player = scp049;
            Ragdoll = ragdoll;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Ragdoll Ragdoll { get; }
    }
}