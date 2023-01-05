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
    public class DoctorSenseEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="FinishingRecallEventArgs" /> class.
        /// </summary>
        /// <param name="scp049"> <inheritdoc cref="Player" /> </param>
        /// <param name="target"> <inheritdoc cref="Target" /> </param>
        /// <param name="reader"> <inheritdoc cref="Reader" /></param>
        /// <param name="bypassChecks"> <inheritdoc cref="BypassChecks" /></param>
        /// <param name="isAllowed"> <inheritdoc cref="IsAllowed" /></param>
        public DoctorSenseEventArgs(Player scp049, Player target, NetworkReader reader, bool bypassChecks = false, bool isAllowed = true)
        {
            Player = scp049;
            Target = target;
            Reader = reader;
            BypassChecks = bypassChecks;
            IsAllowed = isAllowed;
            Cooldown = 5f;
            Duration = 20f;
        }

        /// <summary>
        /// Scp049 Duration of sense.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Scp049 Sense cooldown
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Ignore NW original checks.
        /// </summary>
        public bool BypassChecks { get; set; }

        /// <summary>
        ///     Gets the player who is currently being targeted.
        /// </summary>
        public Player Target { get; }

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