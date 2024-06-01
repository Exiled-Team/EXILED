// -----------------------------------------------------------------------
// <copyright file="ReportingCheaterEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using API.Features;

    using Interfaces;

    /// <summary>
    /// Contains all information before reporting a cheater.
    /// </summary>
    public class ReportingCheaterEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingCheaterEventArgs" /> class.
        /// </summary>
        /// <param name="issuer">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="serverPort">
        /// <inheritdoc cref="ServerPort" />
        /// </param>
        /// <param name="reason">
        /// <inheritdoc cref="Reason" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ReportingCheaterEventArgs(Player issuer, Player target, int serverPort, string reason, bool isAllowed = true)
        {
            Player = issuer;
            Target = target;
            ServerPort = serverPort;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the targeted player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the server id.
        /// </summary>
        public int ServerPort { get; }

        /// <summary>
        /// Gets or sets the report reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the report will be sent.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the issuing player.
        /// </summary>
        public Player Player { get; }
    }
}