// -----------------------------------------------------------------------
// <copyright file="LocalReportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains information before a report is sent to local administrators.
    /// </summary>
    public class LocalReportingEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalReportingEventArgs" /> class.
        /// </summary>
        /// <param name="issuer">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="reason">
        ///     <inheritdoc cref="Reason" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public LocalReportingEventArgs(Player issuer, Player target, string reason, bool isAllowed = true)
        {
            Player = issuer;
            Target = target;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets the report reason.
        /// </summary>
        public string Reason { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}