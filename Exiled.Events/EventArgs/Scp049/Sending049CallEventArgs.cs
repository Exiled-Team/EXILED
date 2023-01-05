// -----------------------------------------------------------------------
// <copyright file="FinishingRecallEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Mirror;

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-049 finishes recalling a player.
    /// </summary>
    public class Sending049CallEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FinishingRecallEventArgs" /> class.
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
        public Sending049CallEventArgs(Player scp049, NetworkReader reader, float duration = 20f, bool bypassChecks = false, bool isAllowed = true)
        {
            Player = scp049;
            Reader = reader;
            Duration = duration;
            BypassChecks = bypassChecks;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Ignore NW original checks.
        /// </summary>
        public bool BypassChecks { get; set; }

        /// <summary>
        ///  Gets/Sets how long 049 duration during a revive should be.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the player who's getting recalled.
        /// </summary>
        public NetworkReader Reader { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}